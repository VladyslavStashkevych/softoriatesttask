namespace SoftoriaTestTask.Services.ParserService.Infrastructure.Constants;

internal static class ScraperScripts
{
    public const string ParseAndGetStateScript =
        """
        () => {
            // data extraction
            const rows = document.querySelectorAll('tr.cmc-table-row');
            const batch = [];
            const toRemove = [];

            for (const row of rows) {
                const rankEl = row.querySelector('.cmc-table__cell--sort-by__rank');
                const nameEl = row.querySelector('.cmc-table__column-name--name');
                const symbEl = row.querySelector('.cmc-table__cell--sort-by__symbol');
                const mCapEl = row.querySelector('.cmc-table__cell--sort-by__market-cap');
                const voluEl = row.querySelector('.cmc-table__cell--sort-by__volume-24-h');
                const chanEl = row.querySelector('.cmc-table__cell--sort-by__percent-change-24-h');

                // loaded rows only
                if (rankEl && nameEl && mCapEl && mCapEl.innerText.trim() !== '') {
                const marketCap = mCapEl.innerText.replace(/[$,?-]/g, '').trim();
                const volume24h = voluEl.innerText.replace(/[$,?-]/g, '').trim();
                const change24h = chanEl.innerText.replace(/[,?%<>-]/g, '').trim();
                    batch.push({
                        rank: rankEl.innerText.trim(),
                        name: nameEl.innerText.trim(),
                        symbol: symbEl.innerText.trim(),
                        marketCap: marketCap == '' ? '0' : marketCap,
                        volume24h: volume24h == '' ? '0' : volume24h,
                        change24h: change24h == '' ? '0' : change24h
                    });
                    toRemove.push(row);
                }
            }
            
            // clean processed rows
            toRemove.forEach(r => r.remove());

            // get state of page
            let nextState = 0;
            const remainingRows = document.querySelectorAll('tr.cmc-table-row');
            
            // check for unloaded rows
            const hasSkeleton = Array.from(remainingRows).some(r => 
                Array.from(r.querySelectorAll('td')).some(td => td.innerText.trim() === '')
            );

            if (hasSkeleton) {
                nextState = 1; // Skeleton exists
            } else if (Array.from(document.querySelectorAll('button'))
                   .some(b => b.textContent?.includes('Load More'))) {
                nextState = 2; // Button exists
            }

            return { batch, nextState };
        }
        """;

    public const string WaitForNewRowsScript =
        """
            () => {
                const rows = document.querySelectorAll('tr.cmc-table-row');
         
                for (const row of rows) {
                    for (const td of row.querySelectorAll('td')) {
                        if (td.textContent.trim() === '') return true;
                    }
                }
                return false;
            }
        """;
    
    public const string FastForwardScript =
        """
        /**
         * @param {number} targetRank - The rank we need to reach before stopping.
         * @returns {Promise<boolean>} - Returns true when the rank is reached.
         */
        async (targetRank) => {
            const getMaxLoadedRank = () => {
                const ranks = Array.from(document.querySelectorAll('.cmc-table__cell--sort-by__rank'));
                if (ranks.length === 0) return 0;
                return Math.max(...ranks.map(r => parseInt(r.innerText.trim()) || 0));
            };

            while (getMaxLoadedRank() < targetRank) {
                const button = Array.from(document.querySelectorAll('button'))
                                    .find(b => b.textContent?.includes('Load More'));
                
                if (!button) break;

                const currentCount = document.querySelectorAll('tr.cmc-table-row').length;
                button.click();

                // Polling wait for new rows to hydrate
                let attempts = 0;
                while (document.querySelectorAll('tr.cmc-table-row').length <= currentCount && attempts < 50) {
                    await new Promise(r => setTimeout(r, 100));
                    attempts++;
                }
            }
            return true;
        };
        """;
}