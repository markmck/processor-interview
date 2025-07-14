import type { CardType } from "../enums/cardType";
import type { TransactionStatus } from "../enums/transactionStatus";

export interface Transaction {
  id: string | number;
  cardNumber?: string;
  cardType?: CardType;
  amount: number;
  timeStamp: string;
  status: TransactionStatus;
}
