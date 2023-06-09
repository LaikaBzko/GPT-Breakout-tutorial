﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
//using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
//using Microsoft.Xna.Framework.Net;
//using Microsoft.Xna.Framework.Storage;

#pragma warning disable 1591 //sadly not yet fully commented

namespace Tutorial_1.RC_Framework
{

    // --------------------------------------- MultiScrollBackGround *****************************************

    public class MultiScrollBackGround : RC_RenderableBounded
    {
        public ScrollBackGround sb1 = null;
        public ScrollBackGround sb2 = null;
        public ScrollBackGround sb3 = null;

        public MultiScrollBackGround(Rectangle boundsZ)
        {
            bounds = new Rectangle(boundsZ.X, boundsZ.Y, boundsZ.Width, boundsZ.Height);
        }

        public void setScrollBackGrounds(ScrollBackGround sbb1, ScrollBackGround sbb2, ScrollBackGround sbb3)
        {
            // set unused ones to null
            sb1 = sbb1;
            sb2 = sbb2;
            sb3 = sbb3;
            if (sb1 != null) sb1.bounds = bounds;
            if (sb2 != null) sb2.bounds = bounds;
            if (sb3 != null) sb3.bounds = bounds;
        }

        public override void Update(GameTime gameTime)
        {
            if (sb1 != null) sb1.Update(gameTime);
            if (sb2 != null) sb2.Update(gameTime);
            if (sb3 != null) sb3.Update(gameTime);
        }

        public override void Draw(SpriteBatch sb)
        {
            if (sb1 != null) sb1.Draw(sb);
            if (sb2 != null) sb2.Draw(sb);
            if (sb3 != null) sb3.Draw(sb);
        }
    }

    // ******************************************************************* MultiRenderable *******************************************************************

    /// <summary>
    /// This class is just up to 4 bounded renderables for coalessing renderables to gethr in more usefull ways, (eg paralax backgrounds)
    /// </summary>
    /// 
    public class MultiRenderable : RC_RenderableBounded
    {
        public RC_RenderableBounded sb1 = null;
        public RC_RenderableBounded sb2 = null;
        public RC_RenderableBounded sb3 = null;
        public RC_RenderableBounded sb4 = null;

        public MultiRenderable(Rectangle boundsZ,
                                   RC_RenderableBounded sbb1, RC_RenderableBounded sbb2,
                                   RC_RenderableBounded sbb3, RC_RenderableBounded sbb4)
        {
            // set unused ones to null 
            bounds = new Rectangle(boundsZ.X, boundsZ.Y, boundsZ.Width, boundsZ.Height);
            sb1 = sbb1;
            sb2 = sbb2;
            sb3 = sbb3;
            sb4 = sbb4;
            if (sb1 != null) sb1.bounds = bounds;
            if (sb2 != null) sb2.bounds = bounds;
            if (sb3 != null) sb3.bounds = bounds;
            if (sb4 != null) sb4.bounds = bounds;
        }

        public void setBounds(Rectangle boundsZ)
        {
            bounds = new Rectangle(boundsZ.X, boundsZ.Y, boundsZ.Width, boundsZ.Height);
            if (sb1 != null) sb1.bounds = bounds;
            if (sb2 != null) sb2.bounds = bounds;
            if (sb3 != null) sb3.bounds = bounds;
            if (sb4 != null) sb4.bounds = bounds;
        }


        public override void Update(GameTime gameTime)
        {
            if (sb1 != null) sb1.Update(gameTime);
            if (sb2 != null) sb2.Update(gameTime);
            if (sb3 != null) sb3.Update(gameTime);
            if (sb4 != null) sb4.Update(gameTime);
        }

        public override void Draw(SpriteBatch sb)
        {
            if (sb1 != null) sb1.Draw(sb);
            if (sb2 != null) sb2.Draw(sb);
            if (sb3 != null) sb3.Draw(sb);
            if (sb4 != null) sb4.Draw(sb);
        }
    }


}

