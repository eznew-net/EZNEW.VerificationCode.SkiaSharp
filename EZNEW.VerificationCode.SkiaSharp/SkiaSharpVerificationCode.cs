using System;
using SkiaSharp;
using EZNEW.Drawing.VerificationCode;

namespace EZNEW.VerificationCode.SkiaSharp
{
    public class SkiaSharpVerificationCode : VerificationCodeProvider
    {
        #region Create code

        /// <summary>
        /// Create code
        /// </summary>
        /// <returns>image binary</returns>
        public override VerificationCodeValue CreateCode()
        {
            byte[] codeImageBinary = new byte[0];
            string codeString = CreateCodeString();
            if (!string.IsNullOrWhiteSpace(codeString))
            {
                #region Generate image

                int width = (fontSize + spaceBetween) * length + 5;
                int height = fontSize + 11;
                using (SKBitmap bitmap = new SKBitmap(width, height))
                {
                    using (SKCanvas canvas = new SKCanvas(bitmap))
                    {
                        canvas.Clear();
                        canvas.DrawColor(new SKColor(backgroundColor.R, backgroundColor.G, backgroundColor.B, backgroundColor.A));
                        using (var paint = new SKPaint())
                        {
                            if (fontColor.HasValue)
                            {
                                var fcolor = fontColor.Value;
                                paint.Color = new SKColor(fcolor.R, fcolor.G, fcolor.B, fcolor.A);
                            }
                            if (!string.IsNullOrWhiteSpace(frontFamilyName))
                            {
                                paint.Typeface = SKTypeface.FromFamilyName(frontFamilyName);
                            }
                            for (int n = 0; n < length; n++)
                            {
                                if (!fontColor.HasValue)
                                {
                                    paint.Color = new SKColor((byte)random.Next(0, 255), (byte)random.Next(0, 255), (byte)random.Next(0, 255));
                                }
                                paint.TextSize = fontSize;
                                canvas.DrawText(codeString[n].ToString(), n * fontSize + spaceBetween, fontSize, paint);
                            }
                            //interfere line
                            if (interfereColor.HasValue)
                            {
                                var lineColor = interfereColor.Value;
                                paint.Color = new SKColor(lineColor.R, lineColor.G, lineColor.B, lineColor.A);
                            }
                            int linePointHalf = width / 2;
                            for (int i = 0; i < interfereNum; i++)
                            {
                                if (!interfereColor.HasValue)
                                {
                                    paint.Color = new SKColor((byte)random.Next(0, 255), (byte)random.Next(0, 255), (byte)random.Next(0, 255));
                                }
                                canvas.DrawLine(random.Next(0, linePointHalf), random.Next(1, height), random.Next(linePointHalf + 1, width), random.Next(1, height), paint);
                            }
                        }
                        using (var image = SKImage.FromBitmap(bitmap))
                        {
                            using (var skdata = image.Encode(SKEncodedImageFormat.Png, 100))
                            {
                                codeImageBinary = skdata.ToArray();
                            }
                        }
                    }
                }

                #endregion
            }

            return new VerificationCodeValue() 
            {
                Code=codeString,
                FileBytes=codeImageBinary
            };
        }

        #endregion

        #region Create code string

        /// <summary>
        /// Create code string
        /// </summary>
        /// <returns>code string</returns>
        private string CreateCodeString()
        {
            string newCode = string.Empty;
            if (charArray == null || charArray.Length == 0)
            {
                return newCode;
            }
            int beginIndexNumber = GetBeginIndexNumber();//开始索引编号
            int endIndexNumber = GetEndIndexNumber();//结束索引编号
            Random random = new Random();
            for (int i = 0; i < length; i++)
            {
                int nowIndex = random.Next(beginIndexNumber, endIndexNumber + 1);
                newCode += charArray[nowIndex].ToString();
            }
            return newCode;
        }

        #endregion

        #region Gets begin index

        /// <summary>
        /// Gets begin index
        /// </summary>
        /// <returns>begin index</returns>
        private int GetBeginIndexNumber()
        {
            int newNumber;
            switch (codeType)
            {
                case VerificationCodeType.Number:
                case VerificationCodeType.NumberAndLetter:
                    newNumber = 0;
                    break;
                case VerificationCodeType.Letter:
                    newNumber = 10;
                    break;
                default:
                    newNumber = 10;
                    break;

            }
            return newNumber;
        }

        #endregion

        #region Gets end index

        /// <summary>
        /// Gets end index
        /// </summary>
        /// <returns>end index</returns>
        private int GetEndIndexNumber()
        {
            int maxNumber = charArray.Length - 1;
            int newNumber;
            switch (codeType)
            {
                case VerificationCodeType.Number:
                    newNumber = 7;
                    break;
                case VerificationCodeType.Letter:
                case VerificationCodeType.NumberAndLetter:
                    newNumber = maxNumber;
                    break;
                default:
                    newNumber = maxNumber;
                    break;
            }
            return newNumber;
        }

        #endregion
    }
}
