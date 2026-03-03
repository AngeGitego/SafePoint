import { onAuthStateChanged } from "firebase/auth";
import { useEffect, useState } from "react";
import { Navigate } from "react-router-dom";
import { auth } from "../firebase";

export default function RequireAuth({ children }) {
  const [user, setUser] = useState(null);
  const [checking, setChecking] = useState(true);

  useEffect(() => {
    const unsub = onAuthStateChanged(auth, (u) => {
      setUser(u);
      setChecking(false);
    });
    return () => unsub();
  }, []);

  if (checking) {
    return (
      <div style={{ color: "white", padding: 24, background: "#1b1530", minHeight: "100vh" }}>
        Checking session...
      </div>
    );
  }

  if (!user) return <Navigate to="/login" replace />;
  return children;
}