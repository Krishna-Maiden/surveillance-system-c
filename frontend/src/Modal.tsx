import React, { ReactNode } from "react";

type ModalProps = {
  open: boolean;
  onClose: () => void;
  children: ReactNode;
};

export const Modal: React.FC<ModalProps> = ({ open, onClose, children }) => {
  if (!open) return null;
  return (
    <div style={{
      position: "fixed", top: 0, left: 0, width: "100vw", height: "100vh",
      background: "rgba(0,0,0,0.3)", display: "flex", alignItems: "center", justifyContent: "center", zIndex: 1000
    }}>
      <div style={{
        background: "#fff", padding: 24, borderRadius: 8, minWidth: 400, minHeight: 200, position: "relative"
      }}>
        <button onClick={onClose} style={{
          position: "absolute", top: 8, right: 8, background: "none", border: "none", fontSize: 20, cursor: "pointer"
        }}>×</button>
        {children}
      </div>
    </div>
  );
};

type ButtonProps = {
  onClick: () => void;
  children: ReactNode;
};

export const Button: React.FC<ButtonProps> = ({ onClick, children }) => (
  <button onClick={onClick} style={{
    marginTop: 16, padding: "8px 16px", background: "#1976d2", color: "#fff", border: "none", borderRadius: 4, cursor: "pointer"
  }}>
    {children}
  </button>
); 