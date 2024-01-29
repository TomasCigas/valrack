using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSpriteController : MonoBehaviour
{
    public static string CHARACTER_PATH = "Images/Characters";
    
    Dictionary<Character,GameObject> characterGameObjectDictionary;
    
    Dictionary<string,Sprite> characterSprites;


    public mapInstance mapInstance{
        get => MapController.Instance.Map;
    }

    // Start is called before the first frame update
    void Start()
    {
        
        LoadSprites();
        
        mapInstance.RegisterCharacterCreated(OnCharacterCreated);

        characterGameObjectDictionary = new Dictionary<Character, GameObject>();

        Character c = mapInstance.CreateCharacter(
            mapInstance.getTileAt(
            mapInstance.Width/2,
            mapInstance.Height/2,
            0
            )
        );

    }

    void LoadSprites(){

        characterSprites = new Dictionary<string,Sprite>();

        Sprite[] wallSprites = Resources.LoadAll<Sprite>(CHARACTER_PATH);

        foreach (Sprite sprite in wallSprites)
        {
            characterSprites.Add(
                sprite.name,
                sprite
            );
            Debug.Log(sprite.name);
        }
    }

    public void OnCharacterCreated(Character character){
        // Create GameObj linked to data of buildObject
        GameObject character_GO = new GameObject();

        characterGameObjectDictionary.Add(character,character_GO);

        character_GO.name = "Character";

        character_GO.transform.position = new Vector3(character.X,character.Y,character.Z);
        character_GO.transform.SetParent(this.transform,true);

        // Add sprite renderer
        SpriteRenderer obj_sr = character_GO.AddComponent<SpriteRenderer>();

        obj_sr.sprite = characterSprites["dude_F"];

        obj_sr.sortingLayerID = SortingLayer.NameToID("Characters");

        // Add callback
        character.RegisterCharacterChangeCallback(OnCharacterChanged);
    }

    void OnCharacterChanged(Character character){
        if(!characterGameObjectDictionary.ContainsKey(character)){
            Debug.LogError("Character is not in direcory");
            return;
        }

        GameObject char_GO = characterGameObjectDictionary[character];

        //Debug.Log("Character pos:"+character.X+","+character.Y+","+character.Z);
        
        char_GO.transform.position = (
            new Vector3(
                character.X,
                character.Y,
                character.Z
            )
        );

    }

}
