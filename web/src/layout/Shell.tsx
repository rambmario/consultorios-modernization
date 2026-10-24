import { NavLink, Outlet } from "react-router-dom";
import { useAuth } from "../auth/AuthContext";
import { navItemsFor } from "../auth/roles";

export function Shell() {
  const { session, logout } = useAuth();
  const items = navItemsFor(session?.roles ?? []);

  return (
    <div className="app-shell">
      <header className="topbar">
        <div>
          <p className="brand">Consultorios</p>
          <p className="muted">{session?.email}</p>
        </div>
        <nav>
          {items.map((item) => (
            <NavLink key={item.to} to={item.to}>
              {item.label}
            </NavLink>
          ))}
        </nav>
        <button type="button" className="ghost" onClick={logout}>
          Salir
        </button>
      </header>
      <main>
        <Outlet />
      </main>
    </div>
  );
}
