const API_BASE_URL = "https://localhost:5001";
const API_VERSION = "v1";

interface AuthResponse {
  token: string;
}

interface TokenData {
  token: string;
  expiresAt: number;
}

class AuthService {
  private tokenData: TokenData | null = null;
  private loginPromise: Promise<string> | null = null;
  private readonly TOKEN_BUFFER_TIME = 120000; // 2 minutes buffer before expiry

  constructor(private apiBaseUrl: string, private apiVersion: string) {}

  async getToken(): Promise<string> {
    if (this.isTokenValid()) {
      return this.tokenData!.token;
    }

    if (this.loginPromise) {
      return this.loginPromise;
    }

    this.loginPromise = this.performLogin();

    try {
      const token = await this.loginPromise;
      return token;
    } finally {
      this.loginPromise = null;
    }
  }

  async forceLogin(): Promise<string> {
    this.clearToken();
    return this.getToken();
  }

  clearToken(): void {
    this.tokenData = null;
    this.loginPromise = null;
  }

  private isTokenValid(): boolean {
    if (!this.tokenData) return false;

    const now = Date.now();
    const expiresWithBuffer = this.tokenData.expiresAt - this.TOKEN_BUFFER_TIME;

    return now < expiresWithBuffer;
  }

  private async performLogin(): Promise<string> {
    try {
      const response = await fetch(`${API_BASE_URL}/${API_VERSION}/auth`, {
        method: "POST",
      });

      if (!response.ok) {
        throw new Error(`Authentication failed: ${response.statusText}`);
      }

      const authData: AuthResponse = await response.json();

      this.tokenData = {
        token: authData.token,
        expiresAt: Date.now() + 43830 * 1000,
      };

      return authData.token;
    } catch (error) {
      console.error("Login failed:", error);
      throw error;
    }
  }
}

// export const authService = new AuthService(
//   process.env.REACT_APP_API_BASE_URL || "https://localhost:44353",
//   process.env.REACT_APP_API_VERSION || "v1"
// );

export const authService = new AuthService("https://localhost:44353", "v1");
