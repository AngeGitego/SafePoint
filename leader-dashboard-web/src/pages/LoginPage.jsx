import { useState } from "react";
import { auth } from "../firebase";
import { useNavigate } from "react-router-dom";
import { signInWithEmailAndPassword, sendPasswordResetEmail } from "firebase/auth";

export default function LoginPage() {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");
  const navigate = useNavigate();
  const [resetMsg, setResetMsg] = useState("");
  const onSubmit = async (e) => {
    e.preventDefault();
    setError("");
    setLoading(true);
    try {
      await signInWithEmailAndPassword(auth, email.trim(), password);
      // Next section: redirect to dashboard
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
    setResetMsg("Password reset email sent ✅ Check your inbox (and spam).");
  } catch (e) {
    setError(e?.message || "Failed to send reset email.");
  }
};

  return (
    <div style={styles.page}>
      <div style={styles.card}>
        <h1 style={styles.title}>Welcome back!</h1>
        <p style={styles.subtitle}>
  Sign in to access your SafePoint Leader Dashboard.
</p>

        <form onSubmit={onSubmit} style={styles.form}>
          <input
            style={styles.input}
            placeholder="Email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            autoComplete="email"
          />

          <input
            style={styles.input}
            placeholder="Password"
            type="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            autoComplete="current-password"
          />

          <button
  style={{
    ...styles.button,
    ...(loading || !email || !password ? styles.buttonDisabled : {}),
  }}
  disabled={loading || !email || !password}
>
  {loading ? "Signing in..." : "Access My Dashboard"}
</button>
<button type="button" onClick={handleResetPassword} style={styles.linkBtn}>
  Forgot Password?
</button>

{resetMsg && <div style={styles.success}>{resetMsg}</div>}
          {error && <p style={styles.error}>{error}</p>}
        </form>

        <p style={styles.note}>
          
        </p>
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
    background:
      "radial-gradient(1200px 600px at 10% 10%, rgba(122,31,162,.25), transparent), #0b0b14",
    color: "white",
  },

  card: {
    width: "100%",
    maxWidth: 440,
    padding: 26,
    borderRadius: 18,
    background: "linear-gradient(180deg, rgba(255,255,255,.06), rgba(255,255,255,.03))",
    border: "1px solid rgba(255,255,255,.10)",
    boxShadow: "0 14px 45px rgba(0,0,0,.55)",
  },

  title: {
    color: "white",
    fontSize: 30,
    margin: 0,
    fontWeight: 900,
    letterSpacing: 0.2,
  },

  subtitle: {
    marginTop: 8,
    marginBottom: 18,
    color: "rgba(255,255,255,.70)",
    fontSize: 13,
    lineHeight: 1.4,
  },

  form: { display: "grid", gap: 12 },

  input: {
  width: "100%",
  padding: "12px 14px",
  borderRadius: 12,
  border: "1px solid rgba(255,255,255,.14)",
  outline: "none",
  background: "rgba(255,255,255,.06)",
  color: "white",
  fontSize: 14,
  boxSizing: "border-box"   // ✅ This fixes the overflow
},
  button: {
  marginTop: 8,
  width: "100%",
  padding: "12px 14px",
  borderRadius: 12,
  border: "1px solid rgba(255,255,255,.10)",
  background: "linear-gradient(180deg, #8B3DFF, #6E2BD9)",
  color: "white",
  fontWeight: 900,
  cursor: "pointer",
  letterSpacing: 0.2,
  boxSizing: "border-box"
},

  buttonDisabled: {
    opacity: 0.55,
    cursor: "not-allowed",
  },

  linkBtn: {
    background: "transparent",
    border: "none",
    color: "rgba(255,255,255,.80)",
    cursor: "pointer",
    marginTop: 10,
    textDecoration: "underline",
    fontWeight: 800,
    justifySelf: "start",
    padding: 0,
  },

  success: {
    marginTop: 10,
    padding: "10px 12px",
    borderRadius: 12,
    background: "rgba(120,255,170,.10)",
    border: "1px solid rgba(120,255,170,.25)",
    color: "rgba(120,255,170,.95)",
    fontSize: 13,
    fontWeight: 800,
  },

  error: {
    marginTop: 10,
    padding: "10px 12px",
    borderRadius: 12,
    background: "rgba(255,140,140,.10)",
    border: "1px solid rgba(255,140,140,.22)",
    color: "#ffb4b4",
    fontSize: 13,
    fontWeight: 800,
  },

  note: {
    color: "rgba(255,255,255,.55)",
    marginTop: 14,
    fontSize: 11,
  },
};