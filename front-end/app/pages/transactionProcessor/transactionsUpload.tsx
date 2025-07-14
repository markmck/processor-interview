import React, { useState } from "react";
import { Upload, FileText, CheckCircle, XCircle } from "lucide-react";
import {
  transactionService,
  TransactionApiError,
} from "../../services/transactionsService";

interface TransactionsUploadProps {
  onUploadSuccess?: () => void;
}

const TransactionsUpload: React.FC<TransactionsUploadProps> = ({
  onUploadSuccess,
}) => {
  const [selectedFile, setSelectedFile] = useState<File | null>(null);
  const [fileContent, setFileContent] = useState<string>("");
  const [isLoading, setIsLoading] = useState(false);
  const [uploadResult, setUploadResult] = useState<{
    success: boolean;
    message: string;
    count?: number;
    fileName?: string;
    errors?: any;
  } | null>(null);

  // Handle file selection and read as text
  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0] || null;
    setSelectedFile(file);
    setUploadResult(null);
    setFileContent("");

    if (file) {
      const reader = new FileReader();
      reader.onload = (event) => {
        if (typeof event.target?.result === "string") {
          setFileContent(event.target.result);
        }
      };
      reader.readAsText(file);
    }
  };

  const handleUpload = async () => {
    if (!selectedFile || !fileContent) {
      setUploadResult({
        success: false,
        message: "Please select a file first",
      });
      return;
    }

    setIsLoading(true);
    setUploadResult(null);

    try {
      const result = await transactionService.uploadTransactions(
        fileContent,
        selectedFile.name
      );
      setSelectedFile(null);
      setFileContent("");

      const fileInput = document.getElementById(
        "transaction-upload-input"
      ) as HTMLInputElement | null;
      if (fileInput) fileInput.value = "";

      setUploadResult({
        success: true,
        message: result.message || "Upload successful",
        count: result.count,
        fileName: selectedFile.name,
      });

      if (onUploadSuccess) onUploadSuccess();
    } catch (error: any) {
      if (error instanceof TransactionApiError) {
        setUploadResult({
          success: false,
          message: error.message,
          errors: error.details,
        });
      } else {
        setUploadResult({
          success: false,
          message: "Network error: " + error.message,
        });
      }
    } finally {
      setIsLoading(false);
    }
  };

  const handleClear = () => {
    setSelectedFile(null);
    setFileContent("");
    setUploadResult(null);
    const fileInput = document.getElementById(
      "transaction-upload-input"
    ) as HTMLInputElement | null;
    if (fileInput) fileInput.value = "";
  };

  return (
    <div className="bg-white rounded-lg shadow p-6">
      <h2 className="text-xl font-semibold mb-4">Upload Transactions</h2>
      <div className="space-y-4">
        <div>
          <label className="block text-sm font-medium text-gray-700 mb-2">
            Select File (JSON, XML, or CSV)
          </label>
          <input
            id="transaction-upload-input"
            type="file"
            accept=".json,.xml,.csv"
            onChange={handleFileChange}
            className="block w-full text-sm text-gray-500 file:mr-4 file:py-2 file:px-4 file:rounded-full file:border-0 file:text-sm file:font-semibold file:bg-blue-50 file:text-blue-700 hover:file:bg-blue-100"
          />
        </div>

        {selectedFile && (
          <div className="bg-gray-50 rounded p-4">
            <div className="flex items-center justify-between mb-2">
              <div className="flex items-center gap-2">
                <FileText size={20} className="text-gray-600" />
                <span className="font-medium">{selectedFile.name}</span>
                <span className="text-sm text-gray-500">
                  ({(selectedFile.size / 1024).toFixed(2)} KB)
                </span>
              </div>
              <button
                onClick={handleClear}
                className="text-sm text-red-600 hover:text-red-800"
              >
                Remove
              </button>
            </div>
            {fileContent && (
              <details className="mt-2">
                <summary className="cursor-pointer text-sm text-blue-600 hover:text-blue-800">
                  Preview file content
                </summary>
                <pre className="mt-2 text-xs bg-white p-2 rounded border overflow-x-auto max-h-48">
                  {fileContent.substring(0, 1000)}
                  {fileContent.length > 1000 && "..."}
                </pre>
              </details>
            )}
          </div>
        )}

        <button
          onClick={handleUpload}
          disabled={!selectedFile || isLoading}
          className="flex items-center gap-2 px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 disabled:bg-gray-400 disabled:cursor-not-allowed transition-colors"
        >
          <Upload size={20} />
          {isLoading ? "Uploading..." : "Upload Transactions"}
        </button>

        {uploadResult && (
          <div
            className={`p-4 rounded-md ${
              uploadResult.success
                ? "bg-green-50 border border-green-200"
                : "bg-red-50 border border-red-200"
            }`}
          >
            <div className="flex items-start gap-2">
              {uploadResult.success ? (
                <CheckCircle className="text-green-600 mt-0.5" size={20} />
              ) : (
                <XCircle className="text-red-600 mt-0.5" size={20} />
              )}
              <div className="flex-1">
                <p
                  className={`font-medium ${
                    uploadResult.success ? "text-green-800" : "text-red-800"
                  }`}
                >
                  {uploadResult.message}
                </p>
                {uploadResult.count && (
                  <p className="text-sm text-gray-600 mt-1">
                    Successfully processed {uploadResult.fileName}.{" "}
                    {uploadResult.count} transactions uploaded.
                  </p>
                )}
                {uploadResult.errors && (
                  <div className="mt-2">
                    <p className="text-sm text-red-700 font-medium">Errors:</p>
                    <pre className="text-xs text-red-600 mt-1 overflow-x-auto">
                      {JSON.stringify(uploadResult.errors, null, 2)}
                    </pre>
                  </div>
                )}
              </div>
            </div>
          </div>
        )}
      </div>
    </div>
  );
};

export default TransactionsUpload;
