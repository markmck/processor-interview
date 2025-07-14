// These should probably be moved to a config file in a real application
const { authService } = await import("../services/authService");

const API_BASE_URL = "https://localhost:5001";
const API_VERSION = "v1";

class TransactionApiError extends Error {
  details: string | null = null;
  status: number;

  constructor(message: string, status: number, details?: string) {
    super(message);
    this.name = "TransactionApiError";
    this.status = status;
    this.details = details ?? null;
  }
}

const handleApiResponse = async (response: Response) => {
  if (!response.ok) {
    const errorData = await response.json().catch(() => ({}));
    throw new TransactionApiError(
      errorData.detail || `HTTP ${response.status}: ${response.statusText}`,
      response.status,
      errorData
    );
  }
  return response.json();
};

export const transactionService = {
  async getTransactions() {
    try {
      const token = await authService.getToken();

      const response = await fetch(
        `${API_BASE_URL}/${API_VERSION}/transactions`,
        {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        }
      );
      return await handleApiResponse(response);
    } catch (error) {
      console.error("Error fetching transactions:", error);
      throw error;
    }
  },

  async uploadTransactions(fileContent: any, fileName: string) {
    const contentType = this.getContentType(fileName);

    if (!contentType) {
      throw new TransactionApiError(
        "Invalid file type. Please upload JSON, XML, or CSV files.",
        400,
        ""
      );
    }

    try {
      const token = await authService.getToken();

      let bodyData = fileContent;
      if (contentType === "application/json" && typeof fileContent !== "string") {
        bodyData = JSON.stringify(fileContent);
      }

      const response = await fetch(
        `${API_BASE_URL}/${API_VERSION}/transactions`,
        {
          method: "POST",
          headers: {
            "Accept": "*/*",
            "Content-Type": contentType,
            Authorization: `Bearer ${token}`,
          },
          body: bodyData,
        }
      );

      return await handleApiResponse(response);
    } catch (error) {
      console.error("Error uploading transactions:", error);
      throw error;
    }
  },

  getContentType(fileName: string): string | null {
    const extension = fileName.split(".").pop()?.toLowerCase();
    switch (extension) {
      case "json":
        return "application/json";
      case "xml":
        return "application/xml";
      case "csv":
        return "text/csv";
      default:
        return null;
    }
  },
};

export { TransactionApiError };
