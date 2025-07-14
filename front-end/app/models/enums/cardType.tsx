export enum CardType {
  Invalid = 0,
  Amex = 3,
  Visa = 4,
  MasterCard = 5,
  Discover = 6,
}

export function getCardTypeName(type: number): string {
    switch (type) {
        case CardType.Amex:
            return "Amex";
        case CardType.Visa:
            return "Visa";
        case CardType.MasterCard:
            return "MasterCard";
        case CardType.Discover:
            return "Discover";
        default:
            return "Invalid";
    }
}