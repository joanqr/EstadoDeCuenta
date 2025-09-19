using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace BBCuentas.Helpers
{
    public class Encripta
    {
        private static string vs_prks = @"?$=EIJREJsdZPC15OvDrhE1uR+MXR9lhG32xQdDjwnY7r1SNkQtJJAWQySSiS0uDdNKlKbnmImw+XAJPjXpD3lYElQ/bKb2CEulFaz9oXxAhhRryxWHCWwwH9H0XhCAgeRrhssK7QZdpl1U0bssjW5vTv8zLIuoQw9G61F+mfnJC9A!;%Qnzi9TIm8afmQHMdmqRqmZml1jqiqwbPfxOhYIkGYlDwDApYeXyNlv5b7C5NgVhbZ0x4dxTBQUurcz1IVGyWTZ'{%QDHrAFb5LOuGO3VnRSR+Txuo4hmUrZrdbn6+CX0UCJtysZnhRXSl/uahFs1uFTCRhP+UxWm8jxblospmhD7yrS(}%Q8I1tbx0SOiXjnoGCBBLRtAT5iiUv6JjVu7I5564bty5gzGFSWh5/4Yr6uf/I0cEfKuKO2aE2FukZWqhK/Mj8y),%wSk7WUK66BqjjpZOyvqMmGssXXEbqL6QIW6+iDT2gV+Sz0kCX6m5dC2+EfTcsWv7q+dz62LoYTdMr6RxvGLbo2].%Qci/bFZjAQCCHYSmpWpJJ4HyJJQm6jbk1LzqDWvUBCtMEmSXautK3G8XKxkn+V4cTVqsXTcmYBLVbGZm+a+q5+[-BAQA*#=sx+xJ1+czjTI0cBFjE1O1ZJLg8vnqfWOOCnfeioh5osuZG0jUQe5EHi02mGGhqzJfJfYD/l+zh9D6HoZo7Kw6NEhRgsPiV36aRMANGcF3rbxWmG5oDsEqhYWw/qW5RBlipHW/VV+McxJtPno2wFU1T84xllA+X3t3UZcwcxAZs1|\";
        private static string vs_pukc = @"?-BAQA*#=Uw5yar+Nox6wlngPZIPvfe0dpHpWNdpR5aEuWv32tB3y9eCbSA8DcZyH9TX2lApmWLT3nWMeu07qMLvJtoF9BgnAmGW76xTV/Wl/YGX1ypyzdVtle3c22Y8+gM5RyPWZ2bB8uqxbmUvmW4GdOYIXapPbydh8LxGTGnAmmhJK9Iy|\";

        public  string RSADecrypt(string DataToDecrypt)
        {
            try
            {
                string vs_decryptedData = "";
                string[] as_chunks = { };

                using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
                {
                    RSA.FromXmlString(GetKV(vs_prks, true));
                    as_chunks = DataToDecrypt.Split(Convert.ToChar(1));

                    foreach (var vs_chunk in as_chunks)
                    {
                        vs_decryptedData += Encoding.UTF8.GetString(RSA.Decrypt(Convert.FromBase64String(vs_chunk), false));
                    }

                }
                return vs_decryptedData;
            }
            catch (CryptographicException e)
            {
                //AppLog(@"ERROR|Error de cifrado de datos. " + e.ToString());
                return null;
            }
        }

        public string RSAEncrypt(string DataToEncrypt)
        {
            try
            {
                var vs_chunks = SplitData(DataToEncrypt);
                string vs_enc_data = "";

                using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
                {
                    string algo = GetKV(vs_pukc, true);
                    RSA.FromXmlString(algo);

                    foreach (var vs_chunk in vs_chunks)
                    {
                        vs_enc_data = vs_enc_data
                                    + Convert.ToChar(1)
                                    + Convert.ToBase64String(RSA.Encrypt(Encoding.UTF8.GetB‌​ytes(vs_chunk), false));
                    }
                    vs_enc_data = vs_enc_data.Trim(Convert.ToChar(1));
                }
                return vs_enc_data;
            }
            catch (CryptographicException e)
            {
                //AppLog(@"ERROR|Error de cifrado de datos. " + e.Message);
                return null;
            }
        }

        private List<string> SplitData(string vs_data)
        {
            int vi_chunk_size = 80; //chars
            int vi_iterations = 0;
            var vs_chunks = new List<string>();

            vi_iterations = (vs_data.Length / vi_chunk_size) + ((vs_data.Length % vi_chunk_size != 0) ? 1 : 0);

            for (int vi_iteration = 1; vi_iteration <= vi_iterations; vi_iteration++)
            {
                vs_chunks.Add((vi_iteration == vi_iterations) ?
                                  vs_data.Substring((vi_iteration - 1) * vi_chunk_size)
                                  :
                                  vs_data.Substring((vi_iteration - 1) * vi_chunk_size, vi_chunk_size)
                             );
            }

            return vs_chunks;
        }

        private string GetKV(string vs_cadena, bool vb_read)
        {
            if (vb_read == false)
            {
                vs_cadena = vs_cadena.Replace(GetVal(0), Convert.ToChar(37).ToString()); // %
                vs_cadena = vs_cadena.Replace(GetVal(1), Convert.ToChar(92).ToString());  // \
                vs_cadena = vs_cadena.Replace(GetVal(2), Convert.ToChar(124).ToString()); // |
                vs_cadena = vs_cadena.Replace(GetVal(3), Convert.ToChar(35).ToString()); // #
                vs_cadena = vs_cadena.Replace(GetVal(4), Convert.ToChar(42).ToString()); // *
                vs_cadena = vs_cadena.Replace(GetVal(5), Convert.ToChar(45).ToString()); // -
                vs_cadena = vs_cadena.Replace(GetVal(6), Convert.ToChar(91).ToString()); // [
                vs_cadena = vs_cadena.Replace(GetVal(7), Convert.ToChar(46).ToString()); // .
                vs_cadena = vs_cadena.Replace(GetVal(8), Convert.ToChar(93).ToString()); // ]
                vs_cadena = vs_cadena.Replace(GetVal(9), Convert.ToChar(44).ToString()); // ,
                vs_cadena = vs_cadena.Replace(GetVal(10), Convert.ToChar(41).ToString()); // )
                vs_cadena = vs_cadena.Replace(GetVal(11), Convert.ToChar(125).ToString()); // }
                vs_cadena = vs_cadena.Replace(GetVal(12), Convert.ToChar(40).ToString()); // (
                vs_cadena = vs_cadena.Replace(GetVal(13), Convert.ToChar(123).ToString()); // {
                vs_cadena = vs_cadena.Replace(GetVal(14), Convert.ToChar(39).ToString()); // '
                vs_cadena = vs_cadena.Replace(GetVal(15), Convert.ToChar(59).ToString()); // ;
                vs_cadena = vs_cadena.Replace(GetVal(16), Convert.ToChar(33).ToString()); // !
                vs_cadena = vs_cadena.Replace(GetVal(17), Convert.ToChar(36).ToString()); // $
                vs_cadena = vs_cadena.Replace(GetVal(18), Convert.ToChar(63).ToString()); // ?

                //Invierte la cadena de caracteres
                char[] va_chars = vs_cadena.ToCharArray();
                Array.Reverse(va_chars);
                return new string(va_chars);
            }
            else
            {
                //Invierte la cadena de caracteres
                char[] va_chars = vs_cadena.ToCharArray();
                Array.Reverse(va_chars);
                vs_cadena = new string(va_chars);

                vs_cadena = vs_cadena.Replace(Convert.ToChar(37).ToString(), GetVal(0)); // %
                vs_cadena = vs_cadena.Replace(Convert.ToChar(92).ToString(), GetVal(1));  // \
                vs_cadena = vs_cadena.Replace(Convert.ToChar(124).ToString(), GetVal(2)); // |
                vs_cadena = vs_cadena.Replace(Convert.ToChar(35).ToString(), GetVal(3)); // #
                vs_cadena = vs_cadena.Replace(Convert.ToChar(42).ToString(), GetVal(4)); // *
                vs_cadena = vs_cadena.Replace(Convert.ToChar(45).ToString(), GetVal(5)); // -
                vs_cadena = vs_cadena.Replace(Convert.ToChar(91).ToString(), GetVal(6)); // [
                vs_cadena = vs_cadena.Replace(Convert.ToChar(46).ToString(), GetVal(7)); // .
                vs_cadena = vs_cadena.Replace(Convert.ToChar(93).ToString(), GetVal(8)); // ]
                vs_cadena = vs_cadena.Replace(Convert.ToChar(44).ToString(), GetVal(9)); // ,
                vs_cadena = vs_cadena.Replace(Convert.ToChar(41).ToString(), GetVal(10)); // )
                vs_cadena = vs_cadena.Replace(Convert.ToChar(125).ToString(), GetVal(11)); // }
                vs_cadena = vs_cadena.Replace(Convert.ToChar(40).ToString(), GetVal(12)); // (
                vs_cadena = vs_cadena.Replace(Convert.ToChar(123).ToString(), GetVal(13)); // {
                vs_cadena = vs_cadena.Replace(Convert.ToChar(39).ToString(), GetVal(14)); // '
                vs_cadena = vs_cadena.Replace(Convert.ToChar(59).ToString(), GetVal(15)); // ;
                vs_cadena = vs_cadena.Replace(Convert.ToChar(33).ToString(), GetVal(16)); // !
                vs_cadena = vs_cadena.Replace(Convert.ToChar(36).ToString(), GetVal(17)); // $
                vs_cadena = vs_cadena.Replace(Convert.ToChar(63).ToString(), GetVal(18)); // ?

                return vs_cadena;
            }

        }

        private string GetVal(int numero)
        {
            string vs_valores = "";
            string vs_valor = "";

            vs_valores += "61A61" + Convert.ToChar(1); //0, ==
            vs_valores += "60A82A83A65A75A101A121A86A97A108A117A101A62" + Convert.ToChar(1); //1, <RSAKeyValue>
            vs_valores += "60A77A111A100A117A108A117A115A62" + Convert.ToChar(1); //2, <Modulus>
            vs_valores += "60A47A77A111A100A117A108A117A115A62" + Convert.ToChar(1); //3, </Modulus>
            vs_valores += "60A69A120A112A111A110A101A110A116A62" + Convert.ToChar(1); //4, <Exponent>
            vs_valores += "60A47A69A120A112A111A110A101A110A116A62" + Convert.ToChar(1); //5, </Exponent>
            vs_valores += "60A80A62" + Convert.ToChar(1); //6, <P>
            vs_valores += "60A47A80A62" + Convert.ToChar(1); //7, </P>
            vs_valores += "60A81A62" + Convert.ToChar(1); //8, <Q>
            vs_valores += "60A47A81A62" + Convert.ToChar(1); //9, </Q>
            vs_valores += "60A68A80A62" + Convert.ToChar(1); //10, <DP>
            vs_valores += "60A47A68A80A62" + Convert.ToChar(1); //11, </DP>
            vs_valores += "60A68A81A62" + Convert.ToChar(1); //12, <DQ>
            vs_valores += "60A47A68A81A62" + Convert.ToChar(1); //13, </DQ>
            vs_valores += "60A73A110A118A101A114A115A101A81A62" + Convert.ToChar(1); //14, <InverseQ>
            vs_valores += "60A47A73A110A118A101A114A115A101A81A62" + Convert.ToChar(1); //15, </InverseQ>
            vs_valores += "60A68A62" + Convert.ToChar(1); //16, <D>
            vs_valores += "60A47A68A62" + Convert.ToChar(1); //17, </D>
            vs_valores += "60A47A82A83A65A75A101A121A86A97A108A117A101A62" + Convert.ToChar(1); //18, </RSAKeyValue>

            string vs_pal = vs_valores.Split(Convert.ToChar(1))[numero];

            for (int i = 0; i < vs_pal.Split('A').Length; i++)
            {
                vs_valor += Convert.ToChar(Convert.ToByte(vs_pal.Split('A')[i])).ToString();
            }

            return vs_valor;
        }

    }
}