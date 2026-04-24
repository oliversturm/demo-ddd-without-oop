export type Money = {
  amount: number;
};

export declare const createMoney: (amount: number) => Money;

export declare const formatMoney: (money: Money) => string;
