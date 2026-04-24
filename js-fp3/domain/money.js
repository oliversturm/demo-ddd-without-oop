/**
 * @typedef {Object} Money
 * @property {number} amount
 */

/**
 * @param {number} amount
 * @returns {Money}
 */
export const createMoney = (amount) => ({ amount });

/**
 * @param {Money} money
 * @returns {string}
 */
export const formatMoney = (money) => money.amount.toFixed(2);
