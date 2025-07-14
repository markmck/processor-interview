// components/features/TransactionsList.jsx
import React from 'react';
import { CheckCircle, XCircle, AlertCircle, CreditCard } from 'lucide-react';
import type { Transaction } from '~/models/responses/transaction';
import { getCardTypeName, type CardType } from '~/models/enums/cardType';
import { getStatusName, type TransactionStatus } from '~/models/enums/transactionStatus';


type TransactionsListProps = {
  transactions: Transaction[];
  isLoading: boolean;
};

const TransactionsList: React.FC<TransactionsListProps> = ({ transactions, isLoading }) => {
  const getCardTypeIcon = (cardType: CardType) => {
    const cardColors = {
      [3]: 'text-green-600',
      [4]: 'text-blue-600',
      [5]: 'text-red-600',
      [6]: 'text-orange-600',
      [0]: 'text-gray-400'
    };
    
    return (
      <div className={`flex items-center gap-1 ${cardColors[cardType] || 'text-gray-400'}`}>
        <CreditCard size={16} />
        <span className="text-sm font-medium">{getCardTypeName(cardType) || 'Unknown'}</span>
      </div>
    );
  };

  const getStatusBadge = (status: TransactionStatus) => {
    const statusConfig: Record<string, { color: string; icon: React.FC<{ size?: number }> }> = {
      [1]: { color: 'bg-green-100 text-green-800', icon: CheckCircle },
      [2]: { color: 'bg-red-100 text-red-800', icon: XCircle },
    };
    
    const config = statusConfig[String(status)];
    const Icon = config.icon;
    
    return (
      <span className={`inline-flex items-center gap-1 px-2 py-1 rounded-full text-xs font-medium ${config.color}`}>
        <Icon size={12} />
        {getStatusName(status)}
      </span>
    );
  };

  const formatCurrency = (amount: number) => {
    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: 'USD'
    }).format(amount);
  };

  const formatDate = (dateString: string) => {
    console.log("Formatting date:", dateString);
    return new Date(dateString).toLocaleString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  };

  if (isLoading) {
    return (
      <div className="bg-white rounded-lg shadow overflow-hidden">
        <div className="px-6 py-4 border-b border-gray-200">
          <h2 className="text-xl font-semibold">Transactions</h2>
        </div>
        <div className="p-8 text-center">
          <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600 mx-auto"></div>
          <p className="mt-4 text-gray-500">Loading transactions...</p>
        </div>
      </div>
    );
  }

  return (
    <div className="bg-white rounded-lg shadow overflow-hidden">
      <div className="px-6 py-4 border-b border-gray-200">
        <h2 className="text-xl font-semibold">Transactions</h2>
        {transactions.length > 0 && (
          <p className="text-sm text-gray-500 mt-1">
            {transactions.length} transaction{transactions.length !== 1 ? 's' : ''} found
          </p>
        )}
      </div>
      
      {transactions.length === 0 ? (
        <div className="p-8 text-center text-gray-500">
          <CreditCard size={48} className="mx-auto mb-4 text-gray-300" />
          <p className="text-lg font-medium mb-2">No transactions found</p>
          <p>Upload a file to get started.</p>
        </div>
      ) : (
        <div className="overflow-x-auto">
          <table className="min-w-full divide-y divide-gray-200">
            <thead className="bg-gray-50">
              <tr>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  ID
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Card Number
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Card Type
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Amount
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Date
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Status
                </th>
              </tr>
            </thead>
            <tbody className="bg-white divide-y divide-gray-200">
              {transactions.map((transaction) => (
                <tr key={transaction.id} className="hover:bg-gray-50">
                  <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">
                    {transaction.id}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                    {transaction.cardNumber}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm">
                    {getCardTypeIcon(transaction.cardType || 0)}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">
                    {formatCurrency(transaction.amount)}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                    {formatDate(transaction.timeStamp)}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap">
                    {getStatusBadge(transaction.status)}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
};

export default TransactionsList;