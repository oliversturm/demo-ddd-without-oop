export const createMoney = (amount) => ({ amount });

export const formatMoney = (money) => money.amount.toFixed(2);
