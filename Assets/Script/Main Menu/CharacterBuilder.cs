using UnityEngine;
using UnityEngine.U2D.Animation;

public class CharacterBuilder : MonoBehaviour
{
    [Header("Modular Parts (Sprite Resolvers)")]
    public SpriteResolver hair;
    public SpriteResolver body;
    public SpriteResolver weapon;

    [Header("Arms")]
    public SpriteResolver upperRightArm;
    public SpriteResolver lowerRightArm;
    public SpriteResolver rightHand;
    public SpriteResolver upperLeftArm;
    public SpriteResolver lowerLeftArm;
    public SpriteResolver leftHand;

    [Header("Legs")]
    public SpriteResolver rightThigh;
    public SpriteResolver rightFoot;
    public SpriteResolver rightFeet;
    public SpriteResolver leftThigh;
    public SpriteResolver leftFoot;
    public SpriteResolver leftFeet;

    [Header("Head")]
    public SpriteResolver head;
    public SpriteResolver neck;

    public void ApplyCharacterData(OwnedCharacterData data)
    {
        if (hair) hair.SetCategoryAndLabel("Hair", data.hairStyle);
        if (body) body.SetCategoryAndLabel("Body", data.bodyStyle);
        if (weapon) weapon.SetCategoryAndLabel("Weapon", data.weaponStyle);

        // Arms
        if (upperRightArm) upperRightArm.SetCategoryAndLabel("UpperRightArm", data.armStyle);
        if (lowerRightArm) lowerRightArm.SetCategoryAndLabel("LowerRightArm", data.armStyle);
        if (rightHand) rightHand.SetCategoryAndLabel("RightHand", data.armStyle);

        if (upperLeftArm) upperLeftArm.SetCategoryAndLabel("UpperLeftArm", data.armStyle);
        if (lowerLeftArm) lowerLeftArm.SetCategoryAndLabel("LowerLeftArm", data.armStyle);
        if (leftHand) leftHand.SetCategoryAndLabel("LeftHand", data.armStyle);

        // Legs
        if (rightThigh) rightThigh.SetCategoryAndLabel("RightThigh", data.legStyle);
        if (rightFoot) rightFoot.SetCategoryAndLabel("RightFoot", data.legStyle);
        if (rightFeet) rightFeet.SetCategoryAndLabel("RightFeet", data.legStyle);

        if (leftThigh) leftThigh.SetCategoryAndLabel("LeftThigh", data.legStyle);
        if (leftFoot) leftFoot.SetCategoryAndLabel("LeftFoot", data.legStyle);
        if (leftFeet) leftFeet.SetCategoryAndLabel("LeftFeet", data.legStyle);

        // Head
        if (head) head.SetCategoryAndLabel("Head", data.headStyle);
        if (neck) neck.SetCategoryAndLabel("Neck", data.headStyle);
    }
}
