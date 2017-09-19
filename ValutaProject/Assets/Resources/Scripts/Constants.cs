using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 各種定数参照用クラス
/// </summary>
public static class Constants
{
    public static int CardCount = 0;

    public static BattleManager BATTLE;
    public static CardButton CARD_BUTTONS;
    public static CardDataUI CARD_DATA_UI;
    public static CardSelectEffect CARD_SELECT_EFFECT;
    public static AnimationEffect ANIMATION_EFFECT;
    public static GameObject ATTACK_ANIMATION;
    public static SoundManager Sound;
    public static EffectTextBox TextBox_Down, TextBox_Up;

    public const int START_HAND = 5;
    public const int START_COST = 5;

    public const string PLAYER_ALIGMENT_PATH = "Sprites/UI/Icon_";

    public const float CardOffsetRatio = 1.5f;
    public static Color S_COLOR = Color.red;
    public const string S_NAME = "剣士";//"剣奏者";
    public const int S_COST = 3;//剣士コスト
    public const string PLAYER_ALIGMENT_SOWRD = PLAYER_ALIGMENT_PATH + "Sowrd";
    public const string UNITZONE_ALIGMENT_SOWRD = PLAYER_ALIGMENT_PATH + "Unit_Sowrd";
    public const string SUPPORTZONE_ALIGMENT_SOWRD = PLAYER_ALIGMENT_PATH + "Support_Sowrd";
    public const string PLAYERSTATE_ALIGMENT_SOWRD = PLAYER_ALIGMENT_PATH + "State_Sowrd";
    public const string COST_ALIGMENT_SOWRD = PLAYER_ALIGMENT_PATH + "Cost_Sowrd";
    
    public static Color M_COLOR = Color.blue;
    public const string M_NAME = "魔術師";
    public const int M_COST = 4;//魔術師コスト
    public const string PLAYER_ALIGMENT_MAGICK = PLAYER_ALIGMENT_PATH + "Magick";
    public const string UNITZONE_ALIGMENT_MAGICK = PLAYER_ALIGMENT_PATH + "Unit_Magick";
    public const string SUPPORTZONE_ALIGMENT_MAGICK = PLAYER_ALIGMENT_PATH + "Support_Magick";
    public const string PLAYERSTATE_ALIGMENT_MAGICK = PLAYER_ALIGMENT_PATH + "State_Magick";
    public const string COST_ALIGMENT_MAGICK = PLAYER_ALIGMENT_PATH + "Cost_Magick";

    public const int ATTACK_COUNT = 1;

    //攻撃優先順位
    public const int TARGET_NOMAL = 1;
    public const int TARGET_GUADIAN = 5;

    //エフェクト名
    public const string CARD_EFFECT_PATH = "Prefabs/Effects/";
    public const string CARD_CAN_SELECT_EFFECT = CARD_EFFECT_PATH + "CanPlayCardEffect";
    public const string CARD_EFFECT_MAGICK_FIRE = CARD_EFFECT_PATH + "Magick_Fire/Magick_Fire";
    public const string CARD_EFFECT_HEAL = CARD_EFFECT_PATH + "Heal/Heal";
    public const string CARD_EFFECT_STATE_UP = CARD_EFFECT_PATH + "StateUp/StateUp";
    public const string CARD_EFFECT_STATE_DOWN = CARD_EFFECT_PATH + "StateDown/StateDown";

    //カード枠Sprite名
    public const string CARD_FRAME_PATH = "Sprites/CardFrame/";
    public const string CARD_FRAME_SWORD = CARD_FRAME_PATH + "Sword";
    public const string CARD_FRAME_SWORD_UNIT = CARD_FRAME_SWORD + "_Unit";
    public const string CARD_FRAME_MAGICK = CARD_FRAME_PATH + "Magician";
    public const string CARD_FRAME_MAGICK_UNIT = CARD_FRAME_MAGICK + "_Unit";
    public const string CARD_FRAME_TRAP = CARD_FRAME_PATH + "Trap";
    public const string CARD_FRAME_LIFE = CARD_FRAME_PATH + "Life";
    public const string CARD_FRAME_BACK = CARD_FRAME_PATH + "Back";
}
