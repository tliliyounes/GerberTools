﻿using GerberLibrary;
using GerberLibrary.Core;
using GerberLibrary.Core.Primitives;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FrontPanelBuilder
{
    public partial class FrontPanelBuilder : Form
    {
        public FrontPanelBuilder()
        {
            InitializeComponent();
        }

        private static void ConvertFile(string a, bool back)
        {
            string basename = a.Substring(0, a.Length - 4);

            string png = basename + "Silk.png";
            string goldpng = basename + "Gold.png";
            if (back)
            {
                png = basename + "BottomSilk.png";
                goldpng = basename + "BottomGold.png";
            }

            Bitmap B = (Bitmap)Image.FromFile(png);
            Bitmap B2 = null;

            if (System.IO.File.Exists(goldpng)) B2 = (Bitmap)Image.FromFile(goldpng);

            try
            {
                System.IO.Directory.CreateDirectory(basename);
            }
            catch (Exception)
            {

            }

            string newa = basename + "\\" + System.IO.Path.GetFileName(a); ;
            string gko = basename + ".gko";
            string pnl = basename + ".pnl";

            if (System.IO.File.Exists(gko) == false && System.IO.File.Exists(pnl) == true)
            {
                gko = pnl;
            }

            string newgko = basename + "\\" + System.IO.Path.GetFileNameWithoutExtension(a) + ".gko"; ;

            if (System.IO.File.Exists(gko))
            {
                System.IO.File.Copy(gko, newgko, true);

            }

            System.IO.File.Copy(a, newa, true);
            a = newa;

            string p = basename + "/topsilk.gto";
            if (back) p = basename + "/bottomsilk.gbo";
            if (back) B.RotateFlip(RotateFlipType.RotateNoneFlipX);
            B.RotateFlip(RotateFlipType.RotateNoneFlipY);
            if (B2 != null) B2.RotateFlip(RotateFlipType.RotateNoneFlipY);
            if (B != null)
            {
                int polyid = 0;

                double Res = 200.0 / 25.4;

                ParsedGerber PLS = null;
                string f = basename + ".gko";
                if (System.IO.File.Exists(f))
                {
                    PLS = PolyLineSet.LoadGerberFile(new StandardConsoleLog(), f);


                    string bottomcopper = basename + "/bottomcopper.gbl";
                    string topcopper = basename + "/topcopper.gtl";
                    string bottomsoldermask = basename + "/bottomsoldermask.gbs";
                    string topsoldermask = basename + "/topsoldermask.gts";
                    string bottomsilk = basename + "/bottomsilk.gbo";
                    if (back) bottomsilk = basename + "/topsilk.gto";
                    GerberArtWriter GAW3 = new GerberLibrary.GerberArtWriter();
                    GAW3.Write(topsoldermask);

                    GerberArtWriter GAW = new GerberLibrary.GerberArtWriter();
                    PolyLine PL = new PolyLine(polyid++);
                    PL.Add(PLS.BoundingBox.TopLeft.X, PLS.BoundingBox.TopLeft.Y);
                    PL.Add(PLS.BoundingBox.BottomRight.X, PLS.BoundingBox.TopLeft.Y);
                    PL.Add(PLS.BoundingBox.BottomRight.X, PLS.BoundingBox.TopLeft.Y + 8);
                    PL.Add(PLS.BoundingBox.TopLeft.X, PLS.BoundingBox.TopLeft.Y + 8);
                    GAW.AddPolygon(PL);


                    PolyLine PL3 = new PolyLine(polyid++);
                    PL3.Add(PLS.BoundingBox.TopLeft.X, PLS.BoundingBox.BottomRight.Y - 8);
                    PL3.Add(PLS.BoundingBox.BottomRight.X, PLS.BoundingBox.BottomRight.Y - 8);
                    PL3.Add(PLS.BoundingBox.BottomRight.X, PLS.BoundingBox.BottomRight.Y);
                    PL3.Add(PLS.BoundingBox.TopLeft.X, PLS.BoundingBox.BottomRight.Y);
                    GAW.AddPolygon(PL3);

                    GAW.Write(bottomsoldermask);


                    GerberArtWriter GAW2 = new GerberLibrary.GerberArtWriter();
                    PolyLine PL2 = new PolyLine(polyid++);
                    PL2.Add(PLS.BoundingBox.TopLeft.X, PLS.BoundingBox.TopLeft.Y);
                    PL2.Add(PLS.BoundingBox.BottomRight.X, PLS.BoundingBox.TopLeft.Y);
                    PL2.Add(PLS.BoundingBox.BottomRight.X, PLS.BoundingBox.BottomRight.Y);
                    PL2.Add(PLS.BoundingBox.TopLeft.X, PLS.BoundingBox.BottomRight.Y);
                    GAW2.AddPolygon(PL2);
                    GAW2.Write(bottomcopper);

                    if (B2 != null)
                    {
                        GerberLibrary.ArtWork.Functions.WriteBitmapToGerber(topcopper, PLS, Res, B2, -128);
                        GerberLibrary.ArtWork.Functions.WriteBitmapToGerber(topsoldermask, PLS, Res, B2, -128);
                    }
                    else
                    {
                        GAW2.Write(topcopper);
                    }



                }

                GerberLibrary.ArtWork.Functions.WriteBitmapToGerber(p, PLS, Res, B, -128);


            }
        }
    }
}
