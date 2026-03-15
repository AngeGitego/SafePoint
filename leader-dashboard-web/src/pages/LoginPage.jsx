import { useState } from "react";
import { auth } from "../firebase";
import { useNavigate } from "react-router-dom";
import { signInWithEmailAndPassword, sendPasswordResetEmail } from "firebase/auth";

export default function LoginPage() {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");
  const [resetMsg, setResetMsg] = useState("");
  const navigate = useNavigate();

  const onSubmit = async (e) => {
    e.preventDefault();
    setError("");
    setResetMsg("");
    setLoading(true);

    try {
      await signInWithEmailAndPassword(auth, email.trim(), password);
      navigate("/dashboard");
    } catch (err) {
      setError(err?.message || "Login failed");
    } finally {
      setLoading(false);
    }
  };

  const handleResetPassword = async () => {
    setError("");
    setResetMsg("");

    if (!email || !email.includes("@")) {
      setError("Enter your email first, then click Forgot Password.");
      return;
    }

    try {
      await sendPasswordResetEmail(auth, email.trim());
      setResetMsg("Password reset email sent. Check your inbox and spam folder.");
    } catch (e) {
      setError(e?.message || "Failed to send reset email.");
    }
  };

  return (
    <div style={styles.page}>
      <div style={styles.card}>
        <div style={styles.brandRow}>
          <div style={styles.brandBadge}>SP</div>
          <div>
            <div style={styles.brandName}>SafePoint</div>
            <div style={styles.brandSub}>Leader Access Portal</div>
          </div>
        </div>

        <h1 style={styles.title}>Welcome back</h1>
        <p style={styles.subtitle}>
          Sign in to access your SafePoint Leader Dashboard and manage community hazard reports.
        </p>

        <form onSubmit={onSubmit} style={styles.form}>
          <div style={styles.fieldGroup}>
            <label style={styles.label}>Email Address</label>
            <input
              style={styles.input}
              placeholder="Enter your email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              autoComplete="email"
              type="email"
            />
          </div>

          <div style={styles.fieldGroup}>
            <label style={styles.label}>Password</label>
            <input
              style={styles.input}
              placeholder="Enter your password"
              type="password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              autoComplete="current-password"
            />
          </div>

          <button
            type="submit"
            style={{
              ...styles.button,
              ...(loading || !email || !password ? styles.buttonDisabled : {}),
            }}
            disabled={loading || !email || !password}
          >
            {loading ? "Signing in..." : "Access Dashboard"}
          </button>

          <button type="button" onClick={handleResetPassword} style={styles.linkBtn}>
            Forgot Password?
          </button>

          {resetMsg && <div style={styles.success}>{resetMsg}</div>}
          {error && <div style={styles.error}>{error}</div>}
        </form>
      </div>
    </div>
  );
}

const styles = {
  page: {
    minHeight: "100vh",
    display: "grid",
    placeItems: "center",
    padding: 24,
    background: "#F6F8FB",
  },

  card: {
    width: "100%",
    maxWidth: 440,
    padding: 28,
    borderRadius: 22,
    background: "#FFFFFF",
    border: "1px solid #E5E7EB",
    boxShadow: "0 18px 45px rgba(15, 23, 42, 0.08)",
  },

  brandRow: {
    display: "flex",
    alignItems: "center",
    gap: 12,
    marginBottom: 18,
  },

  brandBadge: {
    width: 42,
    height: 42,
    borderRadius: 12,
    background: "#EAF4F1",
    color: "#1F6F5B",
    display: "grid",
    placeItems: "center",
    fontWeight: 900,
    fontSize: 14,
    border: "1px solid #CFE7DE",
  },

  brandName: {
    fontSize: 15,
    fontWeight: 800,
    color: "#111827",
    lineHeight: 1.2,
  },

  brandSub: {
    fontSize: 12,
    color: "#6B7280",
    marginTop: 2,
  },

  title: {
    color: "#111827",
    fontSize: 30,
    margin: 0,
    fontWeight: 800,
    letterSpacing: 0.2,
  },

  subtitle: {
    marginTop: 8,
    marginBottom: 20,
    color: "#6B7280",
    fontSize: 14,
    lineHeight: 1.5,
  },

  form: {
    display: "grid",
    gap: 14,
  },

  fieldGroup: {
    display: "grid",
    gap: 6,
  },

  label: {
    fontSize: 12,
    fontWeight: 700,
    color: "#4B5563",
  },

  input: {
    width: "100%",
    padding: "12px 14px",
    borderRadius: 12,
    border: "1px solid #D1D5DB",
    outline: "none",
    background: "#FFFFFF",
    color: "#111827",
    fontSize: 14,
    boxSizing: "border-box",
  },

  button: {
    marginTop: 6,
    width: "100%",
    padding: "12px 14px",
    borderRadius: 12,
    border: "1px solid #1F6F5B",
    background: "#1F6F5B",
    color: "white",
    fontWeight: 800,
    cursor: "pointer",
    letterSpacing: 0.2,
    boxSizing: "border-box",
    boxShadow: "0 6px 14px rgba(31,111,91,0.16)",
  },

  buttonDisabled: {
    opacity: 0.55,
    cursor: "not-allowed",
    boxShadow: "none",
  },

  linkBtn: {
    background: "transparent",
    border: "none",
    color: "#1F6F5B",
    cursor: "pointer",
    marginTop: 2,
    textDecoration: "underline",
    fontWeight: 700,
    justifySelf: "start",
    padding: 0,
  },

  success: {
    marginTop: 6,
    padding: "10px 12px",
    borderRadius: 12,
    background: "#ECFDF5",
    border: "1px solid #A7F3D0",
    color: "#047857",
    fontSize: 13,
    fontWeight: 700,
  },

  error: {
    marginTop: 6,
    padding: "10px 12px",
    borderRadius: 12,
    background: "#FEF2F2",
    border: "1px solid #FECACA",
    color: "#B91C1C",
    fontSize: 13,
    fontWeight: 700,
  },
};