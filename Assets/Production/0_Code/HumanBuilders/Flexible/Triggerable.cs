﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HumanBuilders {
  public interface ITriggerable {
    void Pull();
  }

  [Serializable]
  public abstract class Triggerable : ITriggerable {
    public virtual void Pull() {

    }
  }
}