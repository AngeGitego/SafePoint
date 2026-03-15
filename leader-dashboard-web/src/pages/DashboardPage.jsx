import { useEffect, useMemo, useRef, useState } from "react";
import jsPDF from "jspdf";
import "../dashboard.css";
import { signOut } from "firebase/auth";
import { MapContainer, TileLayer, Marker, Popup } from "react-leaflet";
import {
  collection,
  doc,
  getDocs,
  onSnapshot,
  orderBy,
  query,
  updateDoc,
  where,
} from "firebase/firestore";
import { auth, db } from "../firebase";

// Responsive hook
function useIsNarrow(breakpoint = 960) {
  const [isNarrow, setIsNarrow] = useState(() => window.innerWidth <= breakpoint);

  useEffect(() => {
    const onResize = () => setIsNarrow(window.innerWidth <= breakpoint);
    window.addEventListener("resize", onResize);
    return () => window.removeEventListener("resize", onResize);
  }, [breakpoint]);

  return isNarrow;
}

export default function DashboardPage() {
  const BUILD_STAMP = "SafePoint Dashboard • BUILD 2026-03-02";
  const isNarrow = useIsNarrow(960);

  const [leader, setLeader] = useState(null);
  const [reports, setReports] = useState([]);

  // Filters
  const [statusFilter, setStatusFilter] = useState("ALL");
  const [categoryFilter, setCategoryFilter] = useState("ALL");
  const [search, setSearch] = useState("");

  // Summary
  const [summaryMonth, setSummaryMonth] = useState(getMonthKey(new Date()));

  // UI state
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  // Map/list sync
  const [selectedId, setSelectedId] = useState(null);
  const mapRef = useRef(null);
  const markerRefs = useRef({});
  const reportRefs = useRef({});

  // Modal (Escalate)
  const [escalateOpen, setEscalateOpen] = useState(false);
  const [selectedReport, setSelectedReport] = useState(null);
  const [authorityName, setAuthorityName] = useState("");
  const [leaderComment, setLeaderComment] = useState("");

  const categories = useMemo(() => getCategories(reports), [reports]);

  const filteredReports = useMemo(
    () => applyFilters(reports, statusFilter, categoryFilter, search),
    [reports, statusFilter, categoryFilter, search]
  );

  const monthReports = useMemo(
    () => filterByMonth(reports, summaryMonth),
    [reports, summaryMonth]
  );

  const summary = useMemo(() => buildSummary(monthReports), [monthReports]);
  const mapCenter = useMemo(() => getMapCenter(filteredReports), [filteredReports]);

  useEffect(() => {
    let unsubReports = null;

    const loadLeaderAndReports = async () => {
      setLoading(true);
      setError("");

      try {
        const user = auth.currentUser;
        if (!user?.email) throw new Error("No authenticated user email found.");

        const leaderQuery = query(
          collection(db, "leaders"),
          where("email", "==", user.email)
        );

        const leaderSnap = await getDocs(leaderQuery);

        if (leaderSnap.empty) {
          throw new Error(
            "No leader profile found. Create a matching document in Firestore 'leaders' with this email."
          );
        }

        const leaderData = {
          id: leaderSnap.docs[0].id,
          ...leaderSnap.docs[0].data(),
        };
        setLeader(leaderData);

        const reportsQuery = query(
          collection(db, "reports"),
          where("cellId", "==", leaderData.cellId),
          orderBy("timestamp", "desc")
        );

        unsubReports = onSnapshot(
          reportsQuery,
          (rsnap) => {
            const items = rsnap.docs.map((d) => ({ id: d.id, ...d.data() }));
            setReports(items);
          },
          (err) => setError("Reports listener error: " + err.message)
        );
      } catch (e) {
        setLeader(null);
        setReports([]);
        setError(e?.message || "Failed to load leader profile.");
      } finally {
        setLoading(false);
      }
    };

    loadLeaderAndReports();

    return () => {
      if (unsubReports) unsubReports();
    };
  }, []);

  useEffect(() => {
    if (!mapRef.current) return;

    const valid = filteredReports
      .filter((r) => isFinite(Number(r.lat)) && isFinite(Number(r.lng)))
      .map((r) => [Number(r.lat), Number(r.lng)]);

    if (valid.length >= 2) {
      mapRef.current.fitBounds(valid, { padding: [30, 30] });
    } else if (valid.length === 1) {
      mapRef.current.setView(valid[0], 15);
    }
  }, [filteredReports]);

  useEffect(() => {
    if (!mapRef.current) return;
    const t = setTimeout(() => {
      try {
        mapRef.current.invalidateSize?.();
      } catch {
        // ignore
      }
    }, 250);
    return () => clearTimeout(t);
  }, [isNarrow]);

  const logout = async () => {
    await signOut(auth);
  };

  const resolveWithCommunity = async (reportId) => {
    try {
      await updateDoc(doc(db, "reports", reportId), { status: "COMMUNITY" });
    } catch (e) {
      setError(e?.message || "Failed to resolve report.");
    }
  };

  const openEscalateModal = (report) => {
    setError("");
    setSelectedReport(report);
    setAuthorityName(report.authorityName || "");
    setLeaderComment(report.leaderComment || "");
    setEscalateOpen(true);
  };

  const confirmEscalation = async () => {
    try {
      if (!selectedReport?.id) return;
      if (!authorityName.trim()) {
        setError("Authority name is required to escalate.");
        return;
      }

      await updateDoc(doc(db, "reports", selectedReport.id), {
        status: "ESCALATED",
        authorityName: authorityName.trim(),
        leaderComment: leaderComment.trim(),
      });

      setEscalateOpen(false);
      setSelectedReport(null);
      setAuthorityName("");
      setLeaderComment("");
    } catch (e) {
      setError(e?.message || "Failed to escalate report.");
    }
  };

  const shareWithChat = (report) => {
    try {
      if (!leader?.phone) {
        setError("Leader phone is missing in Firestore leaders.phone");
        return;
      }

      const status = (report.status || "PENDING").toUpperCase();
      const mapsLink = `https://maps.google.com/?q=${report.lat},${report.lng}`;

      const recommendedAction =
        status === "ESCALATED"
          ? `ACTION: Escalate to ${report.authorityName || "relevant authority"}`
          : status === "COMMUNITY"
          ? "ACTION: Fix within community (mobilize residents)."
          : "ACTION: Review and decide (community fix or escalate).";

      const message = [
        "SafePoint Alert 🚧",
        `Category: ${report.category || "Uncategorized"}`,
        `Details: ${report.description || ""}`,
        `Location: ${mapsLink}`,
        `Status: ${status}`,
        recommendedAction,
        report.leaderComment ? `Leader Comment: ${report.leaderComment}` : "",
      ]
        .filter(Boolean)
        .join("\n");

      const url = `https://wa.me/${leader.phone}?text=${encodeURIComponent(message)}`;
      window.open(url, "_blank");
    } catch (e) {
      setError(e?.message || "Failed to share report.");
    }
  };

  const downloadSummaryPdf = () => {
    try {
      if (!leader) {
        setError("Leader profile not loaded yet.");
        return;
      }

      const docPdf = new jsPDF();
      const title = "SafePoint Monthly Summary";
      const monthLabel = summaryMonth || "Selected Month";

      docPdf.setFont("helvetica", "bold");
      docPdf.setFontSize(16);
      docPdf.text(title, 14, 16);

      docPdf.setFont("helvetica", "normal");
      docPdf.setFontSize(11);
      docPdf.text(`Cell: ${leader.cellId || "N/A"}`, 14, 24);
      docPdf.text(`Month: ${monthLabel}`, 14, 30);
      docPdf.text(`Generated: ${new Date().toLocaleString()}`, 14, 36);

      docPdf.setDrawColor(180);
      docPdf.line(14, 40, 196, 40);

      docPdf.setFont("helvetica", "bold");
      docPdf.setFontSize(12);
      docPdf.text("Overview", 14, 48);

      docPdf.setFont("helvetica", "normal");
      docPdf.setFontSize(11);
      docPdf.text(`Total Reports: ${summary.total}`, 14, 56);
      docPdf.text(`Pending: ${summary.statusCounts.PENDING}`, 14, 62);
      docPdf.text(`Resolved (Community): ${summary.statusCounts.COMMUNITY}`, 14, 68);
      docPdf.text(`Escalated: ${summary.statusCounts.ESCALATED}`, 14, 74);

      docPdf.setFont("helvetica", "bold");
      docPdf.setFontSize(12);
      docPdf.text("Top Categories", 14, 86);

      docPdf.setFont("helvetica", "normal");
      docPdf.setFontSize(11);

      let y = 94;

      if (!summary.topCategories.length) {
        docPdf.text("- None", 14, y);
      } else {
        summary.topCategories.forEach((c) => {
          docPdf.text(`- ${c.category}: ${c.count}`, 14, y);
          y += 8;
        });
      }

      docPdf.setFontSize(10);
      docPdf.setTextColor(90);
      docPdf.text("Generated from SafePoint Leader Dashboard", 14, 290);

      const filename = `SafePoint_Summary_${leader.cellId}_${monthLabel}.pdf`;
      docPdf.save(filename);
    } catch (e) {
      setError(e?.message || "Failed to generate PDF.");
    }
  };

  const onReportClick = (r) => {
    setSelectedId(r.id);

    const lat = Number(r.lat);
    const lng = Number(r.lng);

    if (mapRef.current && isFinite(lat) && isFinite(lng)) {
      mapRef.current.setView([lat, lng], 16);
    }

    const marker = markerRefs.current[r.id];
    if (marker && marker.openPopup) marker.openPopup();
  };

  const layoutGridStyle = useMemo(() => {
    if (isNarrow) {
      return {
        display: "grid",
        gridTemplateColumns: "1fr",
        gap: 16,
        alignItems: "start",
      };
    }

    return {
      display: "grid",
      gridTemplateColumns: "420px minmax(0, 1fr)",
      gap: 18,
      alignItems: "start",
    };
  }, [isNarrow]);

  return (
    <div style={styles.page}>
      <header style={styles.header}>
        <div>
          <div style={styles.hTitle}>SafePoint Leader Dashboard</div>
          <div style={styles.hSub}>
            {leader ? (
              <>
                {leader.name} • Cell ID: <b>{leader.cellId}</b>
              </>
            ) : (
              "Loading leader..."
            )}
          </div>
          <div style={styles.buildStamp}>{BUILD_STAMP}</div>
        </div>

        <button onClick={logout} style={styles.logout}>
          Logout
        </button>
      </header>

      <main style={styles.shell}>
        {loading && <div style={styles.panel}>Loading leader profile…</div>}

        {!loading && error && (
          <div style={styles.panel}>
            <div style={{ color: "#B91C1C", fontWeight: 800 }}>Notice</div>
            <div style={{ color: "#374151", marginTop: 8 }}>{error}</div>
          </div>
        )}

        {!loading && leader && !error && (
          <div style={layoutGridStyle}>
            {/* LEFT */}
            <div style={{ display: "grid", gap: 16, minWidth: 0 }}>
              {/* Profile */}
              <div style={styles.panel}>
                <div style={{ display: "flex", justifyContent: "space-between", gap: 12 }}>
                  <div style={{ fontWeight: 800, fontSize: 16, color: "#111827" }}>
                    Leader Profile
                  </div>
                  <div style={{ color: "#6B7280", fontSize: 12 }}>
                    Cell: <b style={{ color: "#111827" }}>{leader.cellId}</b>
                  </div>
                </div>

                <div style={styles.row}>
                  <span style={styles.label}>Name</span> {leader.name}
                </div>
                <div style={styles.row}>
                  <span style={styles.label}>Email</span> {leader.email}
                </div>
                <div style={styles.row}>
                  <span style={styles.label}>Phone</span> {leader.phone}
                </div>
              </div>

              {/* Monthly Summary */}
              <div style={styles.panel}>
                <div style={styles.sectionTitle}>Monthly Summary</div>

                <div style={styles.summaryCard}>
                  <div style={styles.summaryRow}>
                    <label style={styles.summaryLabel}>Select Month</label>
                    <input
                      type="month"
                      value={summaryMonth}
                      onChange={(e) => setSummaryMonth(e.target.value)}
                      style={styles.monthInput}
                    />
                  </div>

                  <div style={styles.summaryStats}>
                    <StatBox number={summary.statusCounts.PENDING} label="Pending" />
                    <StatBox number={summary.statusCounts.COMMUNITY} label="Resolved" />
                    <StatBox number={summary.statusCounts.ESCALATED} label="Escalated" />
                    <StatBox number={summary.total} label="Total" />
                  </div>

                  <div style={styles.summarySubTitle}>Top Categories</div>
                  <div style={{ display: "grid", gap: 6 }}>
                    {summary.topCategories.length === 0 ? (
                      <div style={{ color: "#6B7280" }}>No reports for this month.</div>
                    ) : (
                      summary.topCategories.map((c) => (
                        <div key={c.category} style={styles.categoryRow}>
                          <span style={{ color: "#111827", fontWeight: 800 }}>{c.category}</span>
                          <span style={{ color: "#6B7280" }}>{c.count}</span>
                        </div>
                      ))
                    )}
                  </div>

                  <div style={styles.summaryActions}>
                    <button
                      style={styles.actionBtnOutline}
                      onClick={() =>
                        copyToClipboard(buildSummaryMessage(leader, summaryMonth, summary))
                      }
                    >
                      Copy Summary
                    </button>

                    <button
                      style={styles.actionBtn}
                      onClick={() =>
                        shareSummaryToWhatsApp(leader, summaryMonth, summary, setError)
                      }
                    >
                      Share to WhatsApp
                    </button>

                    <button style={styles.actionBtnOutline} onClick={downloadSummaryPdf}>
                      Download PDF
                    </button>
                  </div>
                </div>
              </div>

              {/* Reports */}
              <div style={styles.panel}>
                <div style={{ display: "flex", justifyContent: "space-between", gap: 12 }}>
                  <div style={styles.sectionTitle}>Reports</div>
                  <div style={{ color: "#6B7280", fontSize: 12 }}>
                    {filteredReports.length} shown
                  </div>
                </div>

                <div style={styles.filtersBar}>
                  <div style={styles.chipsRow}>
                    {["ALL", "PENDING", "COMMUNITY", "ESCALATED"].map((s) => (
                      <button
                        key={s}
                        style={{ ...styles.chip, ...(statusFilter === s ? styles.chipActive : {}) }}
                        onClick={() => setStatusFilter(s)}
                      >
                        {s}
                      </button>
                    ))}
                  </div>

                  <select
                    style={styles.select}
                    value={categoryFilter}
                    onChange={(e) => setCategoryFilter(e.target.value)}
                  >
                    <option value="ALL">All Categories</option>
                    {categories.map((c) => (
                      <option key={c} value={c}>
                        {c}
                      </option>
                    ))}
                  </select>

                  <input
                    style={styles.search}
                    value={search}
                    onChange={(e) => setSearch(e.target.value)}
                    placeholder="Search description..."
                  />
                </div>

                <div style={{ marginTop: 12, display: "grid", gap: 10 }}>
                  {filteredReports.map((r) => (
                    <div
                      key={r.id}
                      ref={(el) => {
                        if (el) reportRefs.current[r.id] = el;
                      }}
                      style={{
                        ...styles.reportCard,
                        ...(selectedId === r.id ? styles.reportCardSelected : {}),
                      }}
                      onClick={() => onReportClick(r)}
                    >
                      <div style={styles.reportTopRow}>
                        <div style={{ color: "#111827", fontWeight: 800 }}>
                          {r.category || "Uncategorized"}
                        </div>
                        <span style={badgeStyle(r.status)}>
                          {(r.status || "PENDING").toUpperCase()}
                        </span>
                      </div>

                      <div style={{ color: "#374151", marginTop: 8 }}>
                        {r.description || "(no description)"}
                      </div>

                      <div style={styles.meta}>
                        {formatTs(r.timestamp)} • {r.lat}, {r.lng}
                      </div>

                      <div style={styles.actionsRow} onClick={(e) => e.stopPropagation()}>
                        <button style={styles.actionBtn} onClick={() => resolveWithCommunity(r.id)}>
                          Resolve
                        </button>
                        <button style={styles.actionBtn} onClick={() => openEscalateModal(r)}>
                          Escalate
                        </button>
                        <button style={styles.actionBtnOutline} onClick={() => shareWithChat(r)}>
                          Share
                        </button>
                      </div>
                    </div>
                  ))}

                  {filteredReports.length === 0 && (
                    <div style={{ color: "#6B7280" }}>No reports match your filters.</div>
                  )}
                </div>
              </div>
            </div>

            {/* RIGHT */}
            <div style={{ display: "grid", gap: 16, minWidth: 0 }}>
              <div style={styles.panel}>
                <div style={styles.sectionTitle}>Map View</div>

                <div
                  style={{
                    ...styles.mapWrap,
                    height: isNarrow ? 380 : 520,
                  }}
                >
                  <MapContainer
                    center={mapCenter}
                    zoom={14}
                    whenCreated={(map) => (mapRef.current = map)}
                    style={{ height: "100%", width: "100%" }}
                  >
                    <TileLayer
                      attribution="&copy; OpenStreetMap contributors"
                      url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
                    />

                    {filteredReports.map((r) => {
                      const pos = [Number(r.lat), Number(r.lng)];
                      if (!isFinite(pos[0]) || !isFinite(pos[1])) return null;

                      return (
                        <Marker
                          key={r.id}
                          position={pos}
                          ref={(ref) => {
                            if (ref) markerRefs.current[r.id] = ref;
                          }}
                          eventHandlers={{
                            click: () => {
                              setSelectedId(r.id);
                              const el = reportRefs.current[r.id];
                              if (el) el.scrollIntoView({ behavior: "smooth", block: "center" });
                            },
                          }}
                        >
                          <Popup>
                            <div style={{ fontWeight: 800 }}>{r.category || "Uncategorized"}</div>
                            <div style={{ marginTop: 4 }}>
                              {(r.status || "PENDING").toUpperCase()}
                            </div>
                            <div style={{ marginTop: 6 }}>{r.description || ""}</div>
                          </Popup>
                        </Marker>
                      );
                    })}
                  </MapContainer>
                </div>
              </div>
            </div>
          </div>
        )}

        {/* Modal */}
        {escalateOpen && (
          <div style={styles.modalBackdrop} onClick={() => setEscalateOpen(false)}>
            <div style={styles.modalCard} onClick={(e) => e.stopPropagation()}>
              <div style={styles.modalTitle}>Escalate to Authority</div>

              <div style={styles.modalLabel}>Authority Name</div>
              <input
                style={styles.modalInput}
                value={authorityName}
                onChange={(e) => setAuthorityName(e.target.value)}
                placeholder="e.g., Sector Officer"
              />

              <div style={styles.modalLabel}>Comment / Reason</div>
              <textarea
                style={styles.modalTextarea}
                value={leaderComment}
                onChange={(e) => setLeaderComment(e.target.value)}
                placeholder="Write a short reason for escalation..."
              />

              <div style={styles.modalActions}>
                <button style={styles.modalBtnOutline} onClick={() => setEscalateOpen(false)}>
                  Cancel
                </button>
                <button style={styles.modalBtn} onClick={confirmEscalation}>
                  Confirm Escalation
                </button>
              </div>
            </div>
          </div>
        )}
      </main>
    </div>
  );
}

function StatBox({ number, label }) {
  return (
    <div style={styles.summaryStatBox}>
      <div style={styles.summaryStatNumber}>{number}</div>
      <div style={styles.summaryStatText}>{label}</div>
    </div>
  );
}

function formatTs(ts) {
  try {
    if (!ts) return "No time";
    const d = ts.toDate ? ts.toDate() : new Date(ts);
    return d.toLocaleString();
  } catch {
    return "Invalid time";
  }
}

function getMapCenter(reports) {
  const first = reports?.find((r) => isFinite(Number(r.lat)) && isFinite(Number(r.lng)));
  if (first) return [Number(first.lat), Number(first.lng)];
  return [-1.9441, 30.0619];
}

function getCategories(reports) {
  const set = new Set();
  (reports || []).forEach((r) => {
    const c = (r.category || "").trim();
    if (c) set.add(c);
  });
  return Array.from(set).sort((a, b) => a.localeCompare(b));
}

function applyFilters(reports, statusFilter, categoryFilter, search) {
  const s = (search || "").trim().toLowerCase();

  return (reports || []).filter((r) => {
    const statusOk =
      statusFilter === "ALL" || (r.status || "PENDING").toUpperCase() === statusFilter;

    const categoryOk =
      categoryFilter === "ALL" || (r.category || "").trim() === categoryFilter;

    const searchOk = !s || (r.description || "").toLowerCase().includes(s);

    return statusOk && categoryOk && searchOk;
  });
}

function badgeStyle(status) {
  const s = (status || "PENDING").toUpperCase();

  const base = {
    padding: "6px 12px",
    borderRadius: 999,
    fontSize: 12,
    fontWeight: 800,
    whiteSpace: "nowrap",
    border: "1px solid transparent",
  };

  if (s === "ESCALATED") {
    return {
      ...base,
      background: "#FEF2F2",
      color: "#B91C1C",
      border: "1px solid #FECACA",
    };
  }

  if (s === "COMMUNITY") {
    return {
      ...base,
      background: "#ECFDF5",
      color: "#047857",
      border: "1px solid #A7F3D0",
    };
  }

  return {
    ...base,
    background: "#FFFBEB",
    color: "#B45309",
    border: "1px solid #FDE68A",
  };
}

function getMonthKey(date) {
  const y = date.getFullYear();
  const m = String(date.getMonth() + 1).padStart(2, "0");
  return `${y}-${m}`;
}

function filterByMonth(reports, monthKey) {
  if (!monthKey) return reports || [];
  const [yStr, mStr] = monthKey.split("-");
  const y = Number(yStr);
  const m = Number(mStr);

  return (reports || []).filter((r) => {
    const d = toJsDate(r.timestamp);
    if (!d) return false;
    return d.getFullYear() === y && d.getMonth() + 1 === m;
  });
}

function toJsDate(ts) {
  try {
    if (!ts) return null;
    if (ts.toDate) return ts.toDate();
    const d = new Date(ts);
    return Number.isNaN(d.getTime()) ? null : d;
  } catch {
    return null;
  }
}

function buildSummary(monthReports) {
  const statusCounts = { PENDING: 0, COMMUNITY: 0, ESCALATED: 0 };
  const categoryCounts = new Map();

  (monthReports || []).forEach((r) => {
    const s = (r.status || "PENDING").toUpperCase();
    if (statusCounts[s] == null) statusCounts[s] = 0;
    statusCounts[s] += 1;

    const c = (r.category || "Uncategorized").trim() || "Uncategorized";
    categoryCounts.set(c, (categoryCounts.get(c) || 0) + 1);
  });

  const topCategories = Array.from(categoryCounts.entries())
    .map(([category, count]) => ({ category, count }))
    .sort((a, b) => b.count - a.count)
    .slice(0, 6);

  const total = (monthReports || []).length;

  return { total, statusCounts, topCategories };
}

function buildSummaryMessage(leader, monthKey, summary) {
  const monthLabel = monthKey || "Selected month";

  const lines = [
    "SafePoint Monthly Summary 📊",
    `Cell: ${leader?.cellId || "N/A"}`,
    `Month: ${monthLabel}`,
    "",
    `Total Reports: ${summary.total}`,
    `Pending: ${summary.statusCounts.PENDING}`,
    `Resolved (Community): ${summary.statusCounts.COMMUNITY}`,
    `Escalated: ${summary.statusCounts.ESCALATED}`,
    "",
    "Top Categories:",
  ];

  if (!summary.topCategories.length) {
    lines.push("- None");
  } else {
    summary.topCategories.forEach((c) => lines.push(`- ${c.category}: ${c.count}`));
  }

  lines.push("");
  lines.push("Shared via SafePoint Leader Dashboard ✅");

  return lines.join("\n");
}

async function copyToClipboard(text) {
  try {
    await navigator.clipboard.writeText(text);
    alert("Summary copied ✅");
  } catch {
    alert("Copy failed. Try manually selecting the text.");
  }
}

function shareSummaryToWhatsApp(leader, monthKey, summary, setError) {
  try {
    if (!leader?.phone) {
      setError?.("Leader phone is missing in Firestore leaders.phone");
      return;
    }

    const message = buildSummaryMessage(leader, monthKey, summary);
    const url = `https://wa.me/${leader.phone}?text=${encodeURIComponent(message)}`;
    window.open(url, "_blank");
  } catch (e) {
    setError?.(e?.message || "Failed to share summary.");
  }
}

const styles = {
  page: {
    minHeight: "100vh",
    background: "#F6F8FB",
    color: "#1F2937",
  },

  header: {
    position: "relative",
    zIndex: 20,
    background: "rgba(255,255,255,0.92)",
    backdropFilter: "blur(10px)",
    borderBottom: "1px solid #E5E7EB",
    padding: "18px 24px",
    display: "flex",
    alignItems: "center",
    justifyContent: "space-between",
  },

  hTitle: {
    color: "#111827",
    fontWeight: 800,
    fontSize: 20,
    letterSpacing: 0.2,
  },

  hSub: {
    color: "#6B7280",
    marginTop: 6,
    fontSize: 13,
  },

  buildStamp: {
    color: "#9CA3AF",
    marginTop: 6,
    fontSize: 11,
  },

  logout: {
    background: "#FFFFFF",
    color: "#1F2937",
    border: "1px solid #D1D5DB",
    padding: "10px 14px",
    borderRadius: 12,
    cursor: "pointer",
    fontWeight: 700,
    boxShadow: "0 1px 2px rgba(0,0,0,0.04)",
  },

  shell: {
    width: "100%",
    maxWidth: "1450px",
    margin: "0 auto",
    padding: "20px 18px 28px",
  },

  panel: {
    background: "#FFFFFF",
    border: "1px solid #E5E7EB",
    borderRadius: 20,
    padding: 18,
    boxShadow: "0 8px 24px rgba(15, 23, 42, 0.06)",
  },

  sectionTitle: {
    marginTop: 0,
    color: "#111827",
    fontWeight: 800,
    fontSize: 15,
  },

  row: {
    color: "#374151",
    marginTop: 10,
    fontSize: 14,
  },

  label: {
    color: "#6B7280",
    marginRight: 10,
    fontWeight: 700,
  },

  reportCard: {
    background: "#FFFFFF",
    border: "1px solid #E5E7EB",
    borderRadius: 16,
    padding: 14,
    cursor: "pointer",
    transition: "all 0.18s ease",
    boxShadow: "0 4px 12px rgba(15, 23, 42, 0.04)",
  },

  reportCardSelected: {
    border: "1px solid #1F6F5B",
    boxShadow: "0 0 0 3px rgba(31,111,91,0.10)",
    background: "#F3FBF8",
  },

  reportTopRow: {
    display: "flex",
    justifyContent: "space-between",
    gap: 12,
    alignItems: "center",
  },

  meta: {
    color: "#6B7280",
    marginTop: 8,
    fontSize: 12,
  },

  actionsRow: {
    display: "flex",
    gap: 10,
    flexWrap: "wrap",
    marginTop: 12,
  },

  actionBtn: {
    background: "#1F6F5B",
    border: "1px solid #1F6F5B",
    color: "white",
    padding: "10px 14px",
    borderRadius: 12,
    cursor: "pointer",
    fontWeight: 800,
    fontSize: 12,
    boxShadow: "0 4px 10px rgba(31,111,91,0.14)",
  },

  actionBtnOutline: {
    background: "#FFFFFF",
    border: "1px solid #D1D5DB",
    color: "#1F2937",
    padding: "10px 14px",
    borderRadius: 12,
    cursor: "pointer",
    fontWeight: 800,
    fontSize: 12,
  },

  mapWrap: {
    width: "100%",
    background: "#F9FAFB",
    border: "1px solid #E5E7EB",
    borderRadius: 18,
    overflow: "hidden",
    marginTop: 12,
    boxShadow: "0 6px 18px rgba(15, 23, 42, 0.05)",
  },

  summaryCard: {
    marginTop: 12,
    background: "#F9FAFB",
    border: "1px solid #E5E7EB",
    borderRadius: 16,
    padding: 14,
  },

  summaryRow: {
    display: "flex",
    justifyContent: "space-between",
    gap: 12,
    alignItems: "center",
  },

  summaryLabel: {
    color: "#4B5563",
    fontWeight: 700,
    fontSize: 12,
  },

  monthInput: {
    background: "#FFFFFF",
    color: "#111827",
    border: "1px solid #D1D5DB",
    borderRadius: 12,
    padding: "10px 12px",
    outline: "none",
  },

  summaryStats: {
    display: "grid",
    gridTemplateColumns: "1fr 1fr",
    gap: 10,
    marginTop: 12,
  },

  summaryStatBox: {
    background: "#FFFFFF",
    border: "1px solid #E5E7EB",
    borderRadius: 14,
    padding: 12,
  },

  summaryStatNumber: {
    fontWeight: 800,
    fontSize: 20,
    color: "#111827",
  },

  summaryStatText: {
    color: "#6B7280",
    fontSize: 12,
    marginTop: 6,
    fontWeight: 700,
  },

  summarySubTitle: {
    marginTop: 14,
    fontWeight: 800,
    fontSize: 13,
    color: "#111827",
  },

  categoryRow: {
    display: "flex",
    justifyContent: "space-between",
    gap: 12,
    padding: "10px 12px",
    borderRadius: 12,
    background: "#FFFFFF",
    border: "1px solid #E5E7EB",
  },

  summaryActions: {
    display: "flex",
    gap: 10,
    flexWrap: "wrap",
    marginTop: 14,
  },

  filtersBar: {
    display: "grid",
    gap: 10,
    marginTop: 12,
  },

  chipsRow: {
    display: "flex",
    gap: 8,
    flexWrap: "wrap",
  },

  chip: {
    padding: "8px 12px",
    borderRadius: 999,
    border: "1px solid #D1D5DB",
    background: "#FFFFFF",
    color: "#374151",
    cursor: "pointer",
    fontWeight: 700,
    fontSize: 12,
  },

  chipActive: {
    background: "#EAF4F1",
    border: "1px solid #B7D9CE",
    color: "#1F6F5B",
  },

  select: {
    width: "100%",
    padding: "10px 12px",
    borderRadius: 12,
    border: "1px solid #D1D5DB",
    background: "#FFFFFF",
    color: "#111827",
    outline: "none",
  },

  search: {
    width: "100%",
    padding: "10px 12px",
    borderRadius: 12,
    border: "1px solid #D1D5DB",
    background: "#FFFFFF",
    color: "#111827",
    outline: "none",
  },

  modalBackdrop: {
    position: "fixed",
    inset: 0,
    background: "rgba(15, 23, 42, 0.35)",
    display: "grid",
    placeItems: "center",
    padding: 16,
    zIndex: 50,
  },

  modalCard: {
    width: "100%",
    maxWidth: 560,
    background: "#FFFFFF",
    borderRadius: 20,
    padding: 18,
    border: "1px solid #E5E7EB",
    boxShadow: "0 20px 60px rgba(15, 23, 42, 0.18)",
  },

  modalTitle: {
    color: "#111827",
    fontWeight: 800,
    fontSize: 18,
    marginBottom: 14,
  },

  modalLabel: {
    color: "#4B5563",
    fontSize: 12,
    fontWeight: 700,
    marginTop: 10,
    marginBottom: 6,
  },

  modalInput: {
    width: "100%",
    padding: "12px 12px",
    borderRadius: 12,
    border: "1px solid #D1D5DB",
    outline: "none",
    background: "#FFFFFF",
    color: "#111827",
    fontSize: 14,
  },

  modalTextarea: {
    width: "100%",
    minHeight: 120,
    padding: "12px 12px",
    borderRadius: 12,
    border: "1px solid #D1D5DB",
    outline: "none",
    background: "#FFFFFF",
    color: "#111827",
    fontSize: 14,
    resize: "vertical",
  },

  modalActions: {
    display: "flex",
    justifyContent: "flex-end",
    gap: 10,
    marginTop: 14,
  },

  modalBtn: {
    background: "#1F6F5B",
    border: "1px solid #1F6F5B",
    color: "white",
    padding: "10px 14px",
    borderRadius: 12,
    cursor: "pointer",
    fontWeight: 800,
  },

  modalBtnOutline: {
    background: "#FFFFFF",
    border: "1px solid #D1D5DB",
    color: "#1F2937",
    padding: "10px 14px",
    borderRadius: 12,
    cursor: "pointer",
    fontWeight: 800,
  },
};