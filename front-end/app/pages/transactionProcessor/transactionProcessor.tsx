// pages/TransactionProcessor.jsx
import React, { useState, useEffect } from 'react';
import TransactionsList from './transactionsList';
import UploadTransactions from './transactionsUpload';
import { transactionService } from '../../services/transactionsService';
import type { Transaction } from '~/models/responses/transaction';

const TransactionProcessor = () => {
  const [transactions, setTransactions] = useState<Transaction[]>([]);
  const [isLoading, setIsLoading] = useState<boolean>(false);
  const [activeTab, setActiveTab] = useState<'transactions' | 'upload'>('transactions');
  const [error, setError] = useState<string | null>(null);

  // Fetch transactions on component mount
  useEffect(() => {
    fetchTransactions();
  }, []);

  const fetchTransactions = async () => {
    setIsLoading(true);
    setError(null);
    
    try {
      const data = await transactionService.getTransactions();
      setTransactions(data);
    } catch (error) {
      console.error('Error fetching transactions:', error);
      setError('Failed to load transactions. Please try again.');
    } finally {
      setIsLoading(false);
    }
  };

  const handleUploadSuccess = () => {
    //fetchTransactions();
    //setActiveTab('transactions');
  };

  type TabButtonProps = {
    tabId: 'transactions' | 'upload';
    label: string;
    isActive: boolean;
    onClick: (tabId: 'transactions' | 'upload') => void;
  };

  const TabButton: React.FC<TabButtonProps> = ({ tabId, label, isActive, onClick }) => (
    <button
      onClick={() => onClick(tabId)}
      className={`py-2 px-1 border-b-2 font-medium text-sm transition-colors ${
        isActive
          ? 'border-blue-500 text-blue-600'
          : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'
      }`}
    >
      {label}
    </button>
  );

  return (
    <div className="min-h-screen bg-gray-50">
      <div className="max-w-7xl mx-auto p-6">
        <div className="mb-8">
          <h1 className="text-3xl font-bold text-gray-900">Transaction Processor</h1>
          <p className="mt-2 text-gray-600">
            Upload and manage your transaction data
          </p>
        </div>
        
        {error && (
          <div className="mb-6 p-4 bg-red-50 border border-red-200 rounded-md">
            <p className="text-red-800">{error}</p>
            <button
              onClick={fetchTransactions}
              className="mt-2 text-sm text-red-600 hover:text-red-800 underline"
            >
              Try again
            </button>
          </div>
        )}
        
        <div className="border-b border-gray-200 mb-6">
          <nav className="-mb-px flex space-x-8">
            <TabButton
              tabId="transactions"
              label="Transactions"
              isActive={activeTab === 'transactions'}
              onClick={setActiveTab}
            />
            <TabButton
              tabId="upload"
              label="Upload"
              isActive={activeTab === 'upload'}
              onClick={setActiveTab}
            />
          </nav>
        </div>

        {activeTab === 'upload' && (
          <UploadTransactions onUploadSuccess={handleUploadSuccess} />
        )}

        {activeTab === 'transactions' && (
          <TransactionsList 
            transactions={transactions} 
            isLoading={isLoading}
          />
        )}
      </div>
    </div>
  );
};

export default TransactionProcessor;