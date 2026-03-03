import { initializeApp } from "firebase/app";
import { getAuth } from "firebase/auth";
import { getFirestore } from "firebase/firestore";

const firebaseConfig = {
  apiKey: "AIzaSyA1R3kRAro2UNLAA7G9nWyV8Q5PNHi5_1g",
  authDomain: "safepoint-ab3f4.firebaseapp.com",
  projectId: "safepoint-ab3f4",
  storageBucket: "safepoint-ab3f4.firebasestorage.app",
  messagingSenderId: "93770216100",
  appId: "1:93770216100:web:082fa3451275ed0ea6ec31",
  measurementId: "G-GZHTLLVY96"

};

const app = initializeApp(firebaseConfig);

export const auth = getAuth(app);
export const db = getFirestore(app);