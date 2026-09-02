# LCTarotCard Developper guide
In this guide I will demonstrate you how to add your custom Tarot card</br>
Warning : General programmong knowledge and lethal company modding knowledge is required to fully understand this tutorial</br>
You can find general C# guide on youtube or on the internet, and you can check out lethal.wiki 's guide on creating a lethal company mod here : https://lethal.wiki/dev/overview


## 1 - Unity project
If you don't already have a Unity project for importing ressources to your mod, you need to create one (I recommend following this tutorial : https://github.com/EvaisaDev/LethalCompanyUnityTemplate/tree/main#readme)</br></br>
Once done, you'll need to add **the image of the front of the tarot card** in Unity </br>
Create a material using the image you've just imported and add the material to your assetbundle, and build your bundle</br>
In your mod project, add the bundle and import your material</br></br>

## 2 - Create the card
To create your card, and make it do stuff, you'll need to do the following :</br>

### Setup the dependency in your mod's main class
For exemple :
```csharp
[BepInPlugin("Your mod's GUID", "Your mod's name", "Your mod's version")]
[BepInDependency("LCTarotCard", "1.1.5")] // Change the version of LCTarotCard mod to the one which you're working with
public class ModMain : BaseUnityPlugin {
    // Your mod's main class stuff here
}
```
Also don't forget to add the dependency in your manifest.json before shipping the mod to thunderstore (so that it'll know you have to have LCTarotCard and will install it automatically)</br>

### Create the card's class
Now that you have imported the required ressources and setup your mod, it's now time to create the actual card</br>
To do that you simply have to create a class which inherit from the class ``LCTarrotCard.Cards.Card.cs``</br>
We will for exemple create a simple card which teleport the player back to the ship, so we first create the class :
```csharp
using GameNetcodeStuff;
using LCTarrotCard.Cards;
using UnityEngine;

namespace Your.Namespace { // Replace with your namespace
    public class TeleportToShipCard : Card { // Replace with your card's class name
        
        public TeleportToShipCard(GameObject cardPrefab, AudioSource audioSource) : base(cardPrefab, audioSource) {
        }

        public override Material GetCardMaterial() {
            throw new System.NotImplementedException();
        }

        public override Material GetCardBurn() {
            throw new System.NotImplementedException();
        }

        public override string ExecuteEffect(PlayerControllerB playerWhoDrew) {
            throw new System.NotImplementedException();
        }

        public override string GetCardName() {
            return "Teleport to ship card"; // Replace with your card's name
        }
    }
}
```
This is what the empty class should look like</br>
Note : You don't have to do anything in the constructor of the class, if you want to setup variable before your card execute go to section ``InitCard Method`` of this guide bellow


### Setting up the materials
The methods ``GetCardMaterial`` and ``GetCardBurn`` are used to provide the material shown on the front of your card when drawn, and the burn effect when the card is consummed</br>
In general you'll want to implement it like this :
```csharp
public override Material GetCardMaterial() {
    return YourAssets.YourCardMaterial; // Replace with your actual material
}

public override Material GetCardBurn() {
    return LCTarrotCard.Ressource.Assets.Materials.BurnWhite;
}
```
You have by default this selection of burn color :
```csharp
public static Material BurnAqua;
public static Material BurnBlue;
public static Material BurnGreen;
public static Material BurnPurple;
public static Material BurnRed;
public static Material BurnWhite;
public static Material BurnYellow;
```
Which can be accessed via ``LCTarrotCard.Ressource.Assets.Materials``</br>
You can also add a custom material from your mod as a burn if you want to

### Executing your card's effect
It is now time to make our card work as intended, with the ExecuteEffect method.</br>
**Important note : The card effect will be executed server side, so you'll need to have setup a networker if you need to make it work accross clients (if you need to send infos to clients)**
If you have never delt with networking before, I recommend this guide to get started : https://lethal.wiki/dev/advanced/networking/messaging</br>
For our exemple of teleporting the player to the ship, we can do it that way :
```csharp
public override string ExecuteEffect(PlayerControllerB playerWhoDrew) {
    if (playerWhoDrew.isPlayerDead || !playerWhoDrew.isPlayerControlled) return "Can't execute effect on dead or non-controlled player.";
    Vector3 InsideShipPosition = StartOfRound.Instance.playerSpawnPositions[0].transform.position; // We're taking here for exemple a spawn position (inside the ship)
    LCTarrotCard.Networker.Instance.TeleportPlayerServerRpc((int)playerWhoDrew.playerClientId, InsideShipPosition, false, true);
    return "You've been teleported back to the ship!";
}
```
Here is a breakdown of what we're doing in the code :
1 - We check that the player is alive and controlled (it should always be but we prefer to make sure)
2 - We get a position inside the ship (here we take one of the spawn point as an exemple)
3 - Teleport the player to that postition
4 - We return a string that indicate about what the card did, which will show up to the player if they enabled the option allowing so
</br>
For this particular exemple we don't have to create a networker because the method teleporting the player already exist in LCTarotCard's networker, but if you need to do something more specific you'll have to setup your own</br>
You can check out [LCTarotCard's networker](https://github.com/Asonyx/LCTarrotCard/blob/master/LCTarrotCard/Networker.cs) which already contains a certain number of action without recoding it yourself</br>
For now it is kind of a mess and not commented, but I'll probably change it soon to make it more readable and usable by others

### Finished class
Here is what our TeleportToShipCard class looks like completed :
```csharp
using GameNetcodeStuff;
using LCTarrotCard.Cards;
using UnityEngine;

namespace Your.Namespace { // Replace with your actual namespace
    public class TeleportToShipCard : Card {
        
        public TeleportToShipCard(GameObject cardPrefab, AudioSource audioSource) : base(cardPrefab, audioSource) {
        }

        public override Material GetCardMaterial() {
            return YourAssets.YourCardMaterial; // Replace with your actual material
        }

        public override Material GetCardBurn() {
            return LCTarrotCard.Ressource.Assets.Materials.BurnWhite;
        }

        public override string ExecuteEffect(PlayerControllerB playerWhoDrew) {
            if (playerWhoDrew.isPlayerDead || !playerWhoDrew.isPlayerControlled) return "Can't execute effect on dead or non-controlled player.";
            Vector3 InsideShipPosition = StartOfRound.Instance.playerSpawnPositions[0].transform.position; // We're taking here for exemple a spawn position (inside the ship)
            LCTarrotCard.Networker.Instance.TeleportPlayerServerRpc((int)playerWhoDrew.playerClientId, InsideShipPosition, false, true);
            return "You've been teleported back to the ship!";
        }

        public override string GetCardName() {
            return "Teleport to Ship Card"; // Replace with your actual card name
        }
    }
}
```

## 3 - Registering your card
Now that we have our functional card, we just need to tell LCTarotCard that we want our card to be added in the deck</br>
We can do it like that :
```csharp
AllCards.RegisterCard(typeof(TeleportToShipCard), 5);
```
As you can see we need to specify our card's class and a weight, the weight is an integer which determine how frequent your card will appear when drawn.</br>
**Note : it is note a percentage. It works by adding all the weights of all the cards together and picking a random number which will be mapped to one card depending on its weight**</br>
You just have to remember that a higher weight lead to a more common card, and so the inverse</br>
To give you an idea, here are the default weights of the default card of LCTarotCard :
```csharp
public static readonly int TowerCard = 20;
public static readonly int WheelCard = 20;
public static readonly int SunCard = 5;
public static readonly int MoonCard = 5;
public static readonly int DevilCard = 10;
public static readonly int HermitCard = 10;
public static readonly int HighPriestessCard = 2;
public static readonly int DeathCard = 10;
public static readonly int HangedManCard = 1;
public static readonly int FoolCard = 17;
```

You should generally register your card at the same time as you load your assets in your mod

## 4 - Advanced method
There are two additional methods you can override if you need to do more specific things with your card

### InitCard Method
```csharp
public override void InitCard(Random random, Material cardBackMaterial = null) {
    base.InitCard(random, cardBackMaterial);
    // Init your variables here
}
```
This method will execute just after the card has been randomly pulled, just before the sliding animation of the card begin</br>
You should use this method if you need to initialize your card **and more importantly if you need to initialize a variable with random**</br>
The method comes with a Random instance as a parameter **that will be synced across clients**</br>
**Note : you shoudn't use this method if you want your effect to be a random effect from a list for exemple, as the execute effect method only execute server side, you should use this method if you need for exemple to randomly change the burn material of the card**

### CardPullingCoroutine
```csharp
public virtual IEnumerator CardPullingCoroutine();
```
This is the most advanced method you can override, and generally you don't need to change something here at all.</br>
This method manage the audio and the material displayed on the card while it is drawn</br>
You can use this method if you want to change the audio, the timings or the way the materials change on the drawn card</br>
If you want to see an exemple of overriding this method, take a look at [the fool card's class](https://github.com/Asonyx/LCTarrotCard/blob/master/LCTarrotCard/Cards/FoolCard.cs)


## Additional note
If you ever need to see more exemple or see how LCTarotCard cards work, you should check out on this github [here](https://github.com/Asonyx/LCTarrotCard/tree/master/LCTarrotCard/Cards)


If you need further help, feel free to DM me on discord : asonyx
