export enum TransactionStatus {
    Accepted = 1,
    Rejected = 2,
}

export function getStatusName(type: number): string {
    switch (type) {
        case TransactionStatus.Accepted:
            return "Accepted";
        case TransactionStatus.Rejected:
            return "Rejected";
        default:
            return "Invalid";
    }
}