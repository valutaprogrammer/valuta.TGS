using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//stack内の処理が全て終わったことを確認する空stack　stackに処理が追加される時、割り込んで一番下に積まれる
public class StackEndBlock : EventBlock
{
    public StackEndBlock(Player caster) : base(caster){ }
}
