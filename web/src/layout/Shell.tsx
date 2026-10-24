import { AppBar, Box, Button, Stack, Toolbar, Typography } from "@mui/material";
import { NavLink, Outlet } from "react-router-dom";
import { useAuth } from "../auth/AuthContext";
import { navItemsFor } from "../auth/roles";

export function Shell() {
  const { session, logout } = useAuth();
  const items = navItemsFor(session?.roles ?? []);

  return (
    <Box>
      <AppBar position="static" color="primary" elevation={0}>
        <Toolbar sx={{ gap: 2, flexWrap: "wrap" }}>
          <Box sx={{ flexGrow: 1 }}>
            <Typography variant="h6" component="p" sx={{ fontFamily: "Georgia, serif" }}>
              Consultorios
            </Typography>
            <Typography variant="body2" sx={{ opacity: 0.85 }}>
              {session?.email}
            </Typography>
          </Box>
          <Stack component="nav" direction="row" spacing={1} sx={{ flexWrap: "wrap" }}>
            {items.map((item) => (
              <Button
                key={item.to}
                component={NavLink}
                to={item.to}
                color="inherit"
                variant="text"
                sx={{
                  "&.active": {
                    textDecoration: "underline",
                    textUnderlineOffset: 4
                  }
                }}
              >
                {item.label}
              </Button>
            ))}
          </Stack>
          <Button color="inherit" variant="outlined" onClick={logout}>
            Salir
          </Button>
        </Toolbar>
      </AppBar>
      <Box component="main" sx={{ maxWidth: 960, mx: "auto", p: 3 }}>
        <Outlet />
      </Box>
    </Box>
  );
}
