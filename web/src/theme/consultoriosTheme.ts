import { createTheme } from "@mui/material/styles";

export const consultoriosTheme = createTheme({
  palette: {
    mode: "light",
    primary: {
      main: "#1f5c4d",
      contrastText: "#f4efe6"
    },
    secondary: {
      main: "#8c5a3c"
    },
    error: {
      main: "#8c2f2f"
    },
    background: {
      default: "#f4efe6",
      paper: "#fffdf8"
    },
    text: {
      primary: "#1d2a24",
      secondary: "#5c6b64"
    },
    divider: "#c9bba8"
  },
  typography: {
    fontFamily: '"Segoe UI", system-ui, sans-serif',
    h1: {
      fontFamily: "Georgia, 'Times New Roman', serif",
      fontSize: "1.75rem",
      fontWeight: 600
    },
    h2: {
      fontFamily: "Georgia, 'Times New Roman', serif",
      fontSize: "1.25rem",
      fontWeight: 600
    },
    button: {
      textTransform: "none",
      fontWeight: 600
    }
  },
  components: {
    MuiButton: {
      defaultProps: {
        variant: "contained"
      }
    },
    MuiTextField: {
      defaultProps: {
        size: "small",
        variant: "outlined"
      }
    },
    MuiAlert: {
      defaultProps: {
        variant: "outlined"
      }
    }
  }
});
