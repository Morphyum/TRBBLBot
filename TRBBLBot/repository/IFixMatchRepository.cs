using TRBBLBot.entity;

namespace TRBBLBot.repository {
    interface IFixMatchRepository {
        void AddFixMatch(FixedMatchEntry entry);
    }
}
