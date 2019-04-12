﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using Microsoft.VisualBasic;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Windows.Forms;

namespace TX_Widget
{
    //用来做设置向导的TabControl.
    //tab竖直。方向在左。
    public class BWVertTabcontrol : TabControl
    {
        public SetFormColor formStryle;
        public BWVertTabcontrol()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw | ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);
            DoubleBuffered = true;
            SizeMode = TabSizeMode.Fixed;
            //ItemSize = new Size(44, 136);
            SetItemSize(new Size(44, 136));
            formStryle = new SetFormColor(SetFormColor.style.jewelryBlue);
        }
        public void SetItemSize(Size size) { ItemSize = size; }

        public bool isDrawTriangle = true;

        protected override void CreateHandle()
        {
            base.CreateHandle();
            Alignment = TabAlignment.Left;
        }

        public Pen ToPen(Color color)
        {
            return new Pen(color);
        }

        public Brush ToBrush(Color color)
        {
            return new SolidBrush(color);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            //Color pageColor = Color.FromArgb(40,40,43);
            //Color tabUnSelectedColor = Color.FromArgb(31, 31,34);
            //Color[] tabSelectedColor = new Color[] { Color.FromArgb(0, 122, 204), Color.FromArgb(0, 122, 204), Color.FromArgb(0, 122, 204) };
            Color pageColor = formStryle.pageColor;
            Color tabUnSelectedColor = formStryle.unSelTabColor;
            Color[] tabSelectedColor = formStryle.selTabColor;
            Bitmap B = new Bitmap(Width, Height);
            Graphics G = Graphics.FromImage(B);  //在B上作图。
            try
            {
                SelectedTab.BackColor = pageColor;  //page颜色
            }
            catch
            {
            }
            G.Clear(pageColor); //page颜色
            G.FillRectangle(new SolidBrush(tabUnSelectedColor), new Rectangle(0, 0, ItemSize.Height + 4, Height));

            //page边框线。
            //G.DrawLine(new Pen(Color.FromArgb(170, 187, 204)), new Point(Width - 1, 0), new Point(Width - 1, Height - 1));
            //G.DrawLine(new Pen(Color.FromArgb(170, 187, 204)), new Point(ItemSize.Height + 1, 0), new Point(Width - 1, 0));
            //G.DrawLine(new Pen(Color.FromArgb(170, 187, 204)), new Point(ItemSize.Height + 3, Height - 1), new Point(Width - 1, Height - 1));
            //G.DrawLine(new Pen(Color.FromArgb(170, 187, 204)), new Point(ItemSize.Height + 3, 0), new Point(ItemSize.Height + 3, 999));
            for (var i = 0; i <= TabCount - 1; i++)
            {
                if (i == SelectedIndex)
                {
                    Rectangle x2 = new Rectangle(new Point(GetTabRect(i).Location.X - 2, GetTabRect(i).Location.Y - 2), new Size(GetTabRect(i).Width + 3, GetTabRect(i).Height - 1));
                    ColorBlend myBlend = new ColorBlend();
                    myBlend.Colors = tabSelectedColor;  //被选中的tab，颜色是渐变的。
                    myBlend.Positions = new[] { 0.0F, 0.5F, 1.0F };
                    LinearGradientBrush lgBrush = new LinearGradientBrush(x2, Color.Black, Color.Black, 90.0F);   //颜色渐变的刷子。
                    lgBrush.InterpolationColors = myBlend;
                    G.FillRectangle(lgBrush, x2);
                    //G.DrawRectangle(new Pen(Color.FromArgb(170, 187, 204)), x2);


                    G.SmoothingMode = SmoothingMode.HighQuality;
                    if (isDrawTriangle)
                    {
                        PointF[] p = new[] { new PointF(ItemSize.Height - 3, (float)(GetTabRect(i).Location.Y + ItemSize.Width / 2.0 - 2)), new PointF(ItemSize.Height + 4, (float)(GetTabRect(i).Location.Y + 1.0 / 3.0 * ItemSize.Width) - 2), 
                        new PointF(ItemSize.Height + 4, (float)(GetTabRect(i).Location.Y + 2.0 / 3.0 * ItemSize.Width) - 2) };
                        G.FillPolygon(new SolidBrush(pageColor), p);  //选项卡切口的三角形
                        //G.DrawPolygon(new Pen(Color.FromArgb(170, 187, 204)), p);   //选项卡切口的三角形
                    }
                    //绘制文字和图片。
                    if (ImageList != null)
                    {
                        try
                        {
                            if (ImageList.Images[TabPages[i].ImageIndex] != null)
                            {
                                G.DrawImage(ImageList.Images[TabPages[i].ImageIndex], new Point(x2.Location.X + 8, x2.Location.Y + 6));
                                G.DrawString("      " + TabPages[i].Text, Font, new SolidBrush(formStryle.selFontColor), x2, new StringFormat() { LineAlignment = StringAlignment.Center, Alignment = StringAlignment.Center });
                            }
                            else
                                G.DrawString(TabPages[i].Text, new Font(Font.FontFamily, Font.Size, FontStyle.Bold), new SolidBrush(formStryle.selFontColor), x2, new StringFormat() { LineAlignment = StringAlignment.Center, Alignment = StringAlignment.Center });
                        }
                        catch (Exception ex)
                        {
                            G.DrawString(TabPages[i].Text, new Font(Font.FontFamily, Font.Size, FontStyle.Bold), new SolidBrush(formStryle.selFontColor), x2, new StringFormat() { LineAlignment = StringAlignment.Center, Alignment = StringAlignment.Center });
                        }
                    }
                    else
                        G.DrawString(TabPages[i].Text, new Font(Font.FontFamily, Font.Size, FontStyle.Bold), new SolidBrush(formStryle.selFontColor), x2, new StringFormat() { LineAlignment = StringAlignment.Center, Alignment = StringAlignment.Center });

                    ////这两个是tab的上下线。
                    //G.DrawLine(new Pen(Color.FromArgb(200, 200, 250)), new Point(x2.Location.X - 1, x2.Location.Y - 1), new Point(x2.Location.X, x2.Location.Y));
                    //G.DrawLine(new Pen(Color.FromArgb(200, 200, 250)), new Point(x2.Location.X - 1, x2.Bottom - 1), new Point(x2.Location.X, x2.Bottom));
                }
                else
                {
                    Rectangle x2 = new Rectangle(new Point(GetTabRect(i).Location.X - 2, GetTabRect(i).Location.Y - 2), new Size(GetTabRect(i).Width + 3, GetTabRect(i).Height + 1));
                    G.FillRectangle(new SolidBrush(tabUnSelectedColor), x2); //未选中的tab的颜色。
                    //G.DrawLine(new Pen(Color.FromArgb(170, 187, 204)), new Point(x2.Right, x2.Top), new Point(x2.Right, x2.Bottom));
                    if (ImageList != null)
                    {
                        try
                        {
                            if (ImageList.Images[TabPages[i].ImageIndex] != null)
                            {
                                G.DrawImage(ImageList.Images[TabPages[i].ImageIndex], new Point(x2.Location.X + 8, x2.Location.Y + 6));
                                G.DrawString("      " + TabPages[i].Text, Font, new SolidBrush(formStryle.fontColor), x2, new StringFormat() { LineAlignment = StringAlignment.Center, Alignment = StringAlignment.Center });
                            }
                            else
                                G.DrawString(TabPages[i].Text, Font, new SolidBrush(formStryle.fontColor), x2, new StringFormat() { LineAlignment = StringAlignment.Center, Alignment = StringAlignment.Center });
                        }
                        catch (Exception ex)
                        {
                            G.DrawString(TabPages[i].Text, Font, new SolidBrush(formStryle.fontColor), x2, new StringFormat() { LineAlignment = StringAlignment.Center, Alignment = StringAlignment.Center });
                        }
                    }
                    else
                        G.DrawString(TabPages[i].Text, Font, new SolidBrush(formStryle.fontColor), x2, new StringFormat() { LineAlignment = StringAlignment.Center, Alignment = StringAlignment.Center });
                }
            }

            e.Graphics.DrawImage((Image)B.Clone(), 0, 0);

            G.Dispose();
            B.Dispose();
        }
    }
}