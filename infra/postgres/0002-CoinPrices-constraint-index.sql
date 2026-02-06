-- 0001-add-unique-CoinPrices.sql
ALTER TABLE "CoinPrices"
ADD CONSTRAINT UQ_Coin_Timestamp UNIQUE ("Symbol", "SavedAt");

CREATE INDEX IX_CoinPrices_Symbol_Timestamp ON "CoinPrices" ("Symbol", "SavedAt");