-- 0001-init-tables.sql
CREATE TABLE IF NOT EXISTS "CoinPrices" (
    "Id" UUID PRIMARY KEY,
    "SavedAt" TIMESTAMP WITH TIME ZONE NOT NULL,
    "Symbol" TEXT NOT NULL,
    "Name" TEXT,
    "Price" DECIMAL NOT NULL,
    "Rank" INTEGER,
    "MarketCap" DECIMAL,
    "Volume24h" DECIMAL,
    "Change24h" DECIMAL
);

CREATE TABLE IF NOT EXISTS "OutboxMessages" (
    "Id" UUID PRIMARY KEY,
    "OccurredOn" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "Payload" TEXT NOT NULL, -- This stores the JSON batch of CoinData
    "IsProcessed" BOOLEAN NOT NULL DEFAULT FALSE
);

-- Index to help the Processor find pending messages quickly
CREATE INDEX IF NOT EXISTS "IX_OutboxMessages_IsProcessed" 
ON "OutboxMessages" ("IsProcessed") 
WHERE "IsProcessed" = FALSE;