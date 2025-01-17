using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;

public class PuzzleCharacterAppearanceContainer : Singleton<PuzzleCharacterAppearanceContainer>
{
    public class CharacterBehaviourBuilder{
        public void UnregistryWhenDestroy(){
            
        }
    }
    public override void OnSingletonInit()
    {
        base.OnSingletonInit();
    }
    public CharacterBehaviourBuilder AddCharacter(){
        return new CharacterBehaviourBuilder();
    }
}
