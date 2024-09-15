# Phasmophobia Tarot Card for Lethal Company
(AKA : LCTarrotCard)

## The Phasmophobia's Tarot Card finally here !
Play with your friends (or alone) and let the tarot card decide your fate !<br/>
Test your luck and see what you can get out of it !

## How to find the tarot ?
- The tarot item will naturally spawn on any moon and count as a scrap as it even has a value if you want to sell it

## How to use the tarot ?
- Simply hold the tarot card in your hand and press the `Left Mouse Button` to draw a card

## Which cards are included ?
Every card from Phasmophobia's tarot deck is included in this mod, which are :
- The Tower
- The Wheel of Fortune
- The Sun
- The Moon
- The Devil
- The Hermit
- The High Priestess
- Death
- The Hanged Man
- The Fool

## What are the probabilities of each card ?
The probabilities of each card are the same as in Phasmophobia, which are :
- The Tower : 20%
- The Wheel of Fortune : 20%
- The Sun : 5%
- The Moon : 5%
- The Devil : 10%
- The Hermit : 10%
- The High Priestess : 2%
- Death : 10%
- The Hanged Man : 1%
- The Fool : 17%

(I will add in the future a way to customize the value via a config file)

## What are the effects of each card ?
### Skip this section if you want to discover the effects by yourself
Here are the different effects you can get from each card :<br/>

<details>
    <summary>The Tower</summary>
    Do a random interaction from the following list :<br/>
    - Open/Close doors<br/>
    - Lock/Unlock security doors<br/>
    - Turn off the breaker<br/>
    - Open/Close the ship's door<br/>
    - Pull the ship's lever
</details>

<details>
    <summary>The Wheel of Fortune</summary>
    Will have a 50/50 chance to do a good or bad effect<br/>
    - Good effect : Restore 20hp<br/>
    - Good effect : Boost some scrap's value by 10%<br/>
    - Bad effect : Damage the player by 20hp<br/>
    - Bad effect : Decrease some scrap's value by 10%<br/>
</details>

<details>
    <summary>The Sun</summary>
    Will do one of the following effects :<br/>
    - Fully restore your health to 100<br/>
    - Boost scrap's value in your inventory by 10%-50%<br/>
    - Boost some scrap's value by 10%-50%<br/>
</details>

<details>
    <summary>The Moon</summary>
    Will do one of the following effects :<br/>
    - Put you at 2hp<br/>
    - Decrease scrap's value in your inventory by 10%-90%<br/>
    - Decrease some scrap's value by 10%-90%<br/>
</details>

<details>
    <summary>The Devil</summary>
    Will do one of the following effects :<br/>
    - Tp a random entity in front of a random player<br/>
    - Blow at a random player<br/>
</details>

<details>
    <summary>The Hermit</summary>
    Will do one of the following effects :<br/>
    - Tp every entity as far away as possible from their current position<br/>
    - Tp the player to a random location inside the facility<br/>
</details>

<details>
    <summary>The High Priestess</summary>
    Will revive a dead player<br/>
    Or if no one is dead will provide an extra chance to the next player who dies by canceling their death and teleporting them to the ship
</details>

<details>
    <summary>Death</summary>
    Will provoke one of the following effects :<br/>
    - Spawn a coilhead if there is none and make it chase a player<br/>
    - Spawn a jester if there is none and pop it<br/>
    - Spawn giant or dog outside<br/>
</details>

<details>
    <summary>The Hanged Man</summary>
    Will instantaneously kill the player who drew the card<br/>
</details>

<details>
    <summary>The Fool</summary>
    Will do nothing<br/>
</details>

## For modders :
If you want to add your own custom card, you can do so by following these steps :<br/>
(Just note that I am going to skip some steps about the base of modding, so if you are new to modding, I recommend you to check the [Lehtal wiki's developer's guide](https://lethal.wiki/dev/overview/))
1. Create a material for the front of the card
2. (Optional) Create a material for the card's burn effect, if you are not satisfied with the default one which are :<br/>
   <details>
    <summary>Default burn colors</summary>
    - Aqua : `Assets.Materials.BurnAqua` <br/>
    - Blue : `Assets.Materials.BurnBlue` <br/>
    - Green : `Assets.Materials.BurnGreen` <br/>
    - Purple : `Assets.Materials.BurnPurple` <br/>
    - Red : `Assets.Materials.BurnRed` <br/>
    - White : `Assets.Materials.BurnWhite` <br/>
    - Yellow : `Assets.Materials.BurnWhite` <br/>
   </details>
3. In your mod's code, create a class that inherite of class `LCTarrotCard.Cards.Card`
4. Implement the methods ``GetCardMaterial`` and ``GetCardBurn`` that return your card front material and burn material
5. Implement the method ``ExecuteEffect(PlayerControllerB playerWhoDrew)`` that will execute when the card is drawn
6. Create a static method somewhere and subscribe it to `LCTarrotCard.Cards.AllCards.OnLoadCard`<br/>
    - In this method (which will be called when every tarot card will be loaded) you will register your card.s by addind them to the dictionnary<br/>
    - To do so, in the method add the following for each card :<br/>
    ``LCTarrotCard.Cards.AllCards.AllCardsWeighted.Add(typeof( YOUR_CARD_CLASS ), YOUR_CARD_PROBABILITY );``

</br>
If you follow these steps you should be able to add your very own card to the game</br>
I would also recommend you to take a look at the mod's code by checking the [GitHub repo](https://github.com/Asonyx/LCTarrotCard/tree/master/) if you want to known more about how that works

# To contact me :
- asonyx (On discord)
