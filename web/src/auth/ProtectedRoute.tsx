import { Navigate, Outlet } from "react-router-dom";
import { useAuth } from "./AuthContext";

export function ProtectedRoute({ roles }: { roles?: readonly string[] }) {
  const { session } = useAuth();
  if (!session) {
    return <Navigate to="/login" replace />;
  }
  if (roles && !roles.some((role) => session.roles.includes(role))) {
    return <Navigate to="/patients" replace />;
  }
  return <Outlet />;
}
