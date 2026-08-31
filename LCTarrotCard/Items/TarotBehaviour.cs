using LCTarrotCard.Config;

namespace LCTarrotCard.Items {
    public class TarotBehaviour : TarotDeckBaseBehaviour {
        
        public override void Awake() {
            base.Awake();
            
            /*Dictionary<Type, int> testCardSet = new Dictionary<Type, int>();
            testCardSet.Add(typeof(FoolCard), 1);
            cardSet = testCardSet;*/
            
            
            drawFoolWhenCantDraw = true;
        }

        public override void Start() {
            base.Start();
            if (!IsOwner) return;
            if (ConfigManager.DeckSize.Value > 0 && ConfigManager.DeckSize.Value <= 100) {
                cardLeft = ConfigManager.DeckSize.Value;
            }
            SetNumberOfCardsServerRpc(cardLeft);
        }

        public override void ItemActivate(bool used, bool buttonDown = true) {
            base.ItemActivate(used, buttonDown);
            PluginLogger.Debug("Trying to pull a card from the following deck (tarot class) : " + cardSet);
        }
    }
}