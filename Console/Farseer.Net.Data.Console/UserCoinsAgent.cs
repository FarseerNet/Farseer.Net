using System.Collections.Generic;

namespace Farseer.Net.Data.Console
{
    public class UserCoinsAgent
    {
        private readonly TestContext _context;

        public UserCoinsAgent(TestContext context)
        {
            _context = context;
        }

        public UserCoinsAgent()
        {
            _context = TestContext.Data;
        }

        public List<UserCoinsPO> ToCreditList()
        {
            return _context.UserCoins.Where(o => o.CoinsType == EumCoinsType.Credit && o.Quota != o.Coins).ToList();
        }
    }
}