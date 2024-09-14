using System.Collections;
using GameNetcodeStuff;
using LCTarrotCard.Ressource;
using UnityEngine;

namespace LCTarrotCard.Cards {
    public class FoolCard : Card {

        private readonly Material[] allCardMat = {
            Assets.Materials.CardTowerMat, 
            Assets.Materials.CardDeathMat, 
            Assets.Materials.CardWheelMat,
            Assets.Materials.CardSunMat,
            Assets.Materials.CardMoonMat,
            Assets.Materials.CardDevilMat,
            Assets.Materials.CardHermitMat,
            Assets.Materials.CardPriestessMat,
            Assets.Materials.CardHangedMat
        };

        private Material _cardToMimic;
        
        public override void InitCard() {
            _cardToMimic = allCardMat[Random.Range(0, allCardMat.Length)];
            base.InitCard();
        }

        public override Material GetCardMaterial() {
            return _cardToMimic;
        }

        public override Material GetCardBurn() {
            return Assets.Materials.BurnPurple;
        }

        public override void ExecuteEffect(PlayerControllerB playerWhoDrew) { }

        public override IEnumerator CardPullingCoroutine() {
            cardAudio.PlayOneShot(Assets.PullCardClips[Random.Range(0, Assets.PullCardClips.Count)]);
            Renderer cardRenderer = cardPrefab.GetComponent<Renderer>();
            yield return new WaitForSeconds(1.8f);
            Material initMat0 = cardRenderer.materials[0];
            Material initMat1 = cardRenderer.materials[1];
            Material initMat2 = cardRenderer.materials[2];

            cardAudio.PlayOneShot(Assets.FoolCardSound);
            
            float delta = 0;
            while (delta < 0.15f) {
                cardRenderer.materials[0].Lerp(initMat0, GetCardBurn(), delta / 0.3f);
                cardRenderer.materials[1].Lerp(initMat1, GetCardBurn(), delta / 0.3f);
                cardRenderer.materials[2].Lerp(initMat2, GetCardBurn(), delta / 0.3f);
                delta += Time.deltaTime;
                yield return null;
            }
            
            Material trMat0 = cardRenderer.materials[0];
            Material trMat1 = cardRenderer.materials[1];
            Material trMat2 = cardRenderer.materials[2];
            
            delta = 0;
            while (delta < 0.15f) {
                cardRenderer.materials[0].Lerp(trMat0, Assets.Materials.CardFoolMat, delta / 0.3f);
                cardRenderer.materials[1].Lerp(trMat1, initMat1, delta / 0.3f);
                cardRenderer.materials[2].Lerp(trMat2, initMat2, delta / 0.3f);
                delta += Time.deltaTime;
                yield return null;
            }
            
            Material[] foolMat = {
                Assets.Materials.CardFoolMat,
                initMat1,
                initMat2
            };

            cardRenderer.materials = foolMat;

            yield return new WaitForSeconds(0.1f);
            
            initMat0 = cardRenderer.materials[0];
            initMat1 = cardRenderer.materials[1];
            initMat2 = cardRenderer.materials[2];
            delta = 0;
            while (delta < 0.3f) {
                cardRenderer.materials[0].Lerp(initMat0, GetCardBurn(), delta / 0.3f);
                cardRenderer.materials[1].Lerp(initMat1, GetCardBurn(), delta / 0.3f);
                cardRenderer.materials[2].Lerp(initMat2, GetCardBurn(), delta / 0.3f);
                delta += Time.deltaTime;
                yield return null;
            }

            yield return new WaitForSeconds(0.2f);
            
            Vector3 initScale = cardPrefab.transform.localScale;
            float delta1 = 0;
            
            while (delta1 < 0.3f) {
                cardPrefab.transform.localScale = initScale * (1 - ControlCurve.Evaluate(delta1 / 0.3f));
                delta1 += Time.deltaTime;
                yield return null;
            }
            
            cardPrefab.transform.localScale = Vector3.zero;
        }

        public FoolCard(GameObject cardPrefab, AudioSource audioSource) : base(cardPrefab, audioSource) { }
    }
}