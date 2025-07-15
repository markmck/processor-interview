import type { Route } from "./+types/home";
import TransactionProcessor from "~/pages/transactionProcessor/transactionProcessor";

export function meta({}: Route.MetaArgs) {
  return [
    { title: "Transaction Processor" },
    { name: "description", content: "Welcome to the transaction processor. Now get to processing!" },
  ];
}

export default function Home() {
  return <TransactionProcessor />;
}
