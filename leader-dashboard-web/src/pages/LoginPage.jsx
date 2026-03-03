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

          <button style={styles.button} disabled={loading || !email || !password}>
            {loading ? "Signing in..." : "Access My Dashboard"}
          </button>
<button type="button" onClick={handleResetPassword} style={styles.linkBtn}>
  Forgot Password?
</button>

{resetMsg && <div style={styles.success}>{resetMsg}</div>}
          {error && <p style={styles.error}>{error}</p>}
        </form>

        <p style={styles.note}>
          Prototype login using Firebase Email/Password.
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
    background: "#1b1530",
    padding: 24,
  },
  card: {
    width: "100%",
    maxWidth: 420,
    background: "#4a2e2e",
    borderRadius: 18,
    padding: 24,
    boxShadow: "0 12px 40px rgba(0,0,0,.35)",
  },
  title: {
    color: "white",
    fontSize: 32,
    margin: "0 0 18px 0",
    fontWeight: 700,
  },
  form: { display: "grid", gap: 12 },
  input: {
    width: "100%",
    padding: "12px 14px",
    borderRadius: 10,
    border: "1px solid rgba(255,255,255,.1)",
    outline: "none",
    background: "#1f1f1f",
    color: "white",
    fontSize: 14,
  },
  linkBtn: {
  background: "transparent",
  border: "none",
  color: "rgba(255,255,255,.85)",
  cursor: "pointer",
  marginTop: 10,
  textDecoration: "underline",
  fontWeight: 700,
},

success: {
  color: "rgba(120,255,170,.9)",
  marginTop: 10,
  fontSize: 13,
  fontWeight: 700,
},
  button: {
    marginTop: 8,
    width: "100%",
    padding: "12px 14px",
    borderRadius: 10,
    border: "none",
    background: "#7A1FA2",
    color: "white",
    fontWeight: 700,
    cursor: "pointer",
  },
  error: { color: "#ffb4b4", margin: "6px 0 0 0", fontSize: 13 },
  note: { color: "rgba(255,255,255,.75)", marginTop: 14, fontSize: 12 },
};