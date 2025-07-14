import { useQuery } from '@tanstack/react-query';
import { transactionService } from '../services/transactionsService';

export const useGetOrders = (request: any) => {
  return useQuery({
    queryKey: ['query', request],
    queryFn: async () => {
      const response = await transactionService.getTransactions();
      return response;
    },
  });
};
