using System.Collections;
using GameNetcodeStuff;
using LCTarrotCard.Ressource;
using UnityEngine;

namespace LCTarrotCard.Cards {
    
    public abstract class Card {
        
        protected static readonly AnimationCurve ControlCurve = new AnimationCurve(
            new Keyframe(0f, 0f, 0f, 0f, .5f, .5f),
            new Keyframe(1f, 1f, 0f, 0f, 0f, 0f));
        
        protected Card(GameObject cardPrefab, AudioSource audioSource) {
            this.cardPrefab = cardPrefab;
            this.cardAudio = audioSource;
        }

        protected readonly AudioSource cardAudio;
        protected readonly GameObject cardPrefab;

        public virtual void InitCard(System.Random random) {
            Renderer renderer = cardPrefab.GetComponent<Renderer>();
            Material[] updatedMats = renderer.materials;
            updatedMats[0] = GetCardMaterial();
            renderer.materials = updatedMats;
            
        }
        public abstract Material GetCardMaterial();
        public abstract Material GetCardBurn();
        public abstract string ExecuteEffect(PlayerControllerB playerWhoDrew);
        public abstract string GetCardName();

        public virtual IEnumerator CardPullingCoroutine() {
            cardAudio.PlayOneShot(Assets.PullCardClips[Random.Range(0, Assets.PullCardClips.Count)]);
            yield return new WaitForSeconds(2.2f);
            Renderer cardRenderer = cardPrefab.GetComponent<Renderer>();
            Material initMat0 = cardRenderer.materials[0];
            Material initMat1 = cardRenderer.materials[1];
            Material initMat2 = cardRenderer.materials[2];
            float delta = 0;
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

    }
}