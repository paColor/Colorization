using NLog;
using System;
using System.Collections.Generic;
using System.Text;

namespace ColorLib.Dierese
{
    public static class AnalyseDierese
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Cherche les diérèses dans le texte <c>tt</c>. Les syllabes des mots concernés sont
        /// modifiées en conséquence.
        /// </summary>
        /// <param name="tt">Le texte du poème. Non <c>null</c>.</param>
        /// <param name="pwL">La liste des <c>PhonWord</c> du poème. Les syllabes ont déjà été
        /// calculées.</param>
        /// <param name="nbrPieds">Le nombre de pieds des vers du poème. 0 si on se contente de la 
        /// détection automatique du nombre de peids voulu.</param>
        public static void ChercheDierese(TheText tt, List<PhonWord> pwL, int nbrPieds)
        {
            logger.ConditionalDebug("ChercheDierese, nbrPieds: {0}", nbrPieds);
            if (tt == null)
            {
                const string Message = "ChercheDierese: tt ne peut pas être null";
                logger.Fatal(Message);
                throw new ArgumentNullException(nameof(tt), Message);
            }
            // créer les zones
            List<ZonePoeme> zL = new List<ZonePoeme>();
            ZonePoeme zpCourante = new ZonePoeme();
            zL.Add(zpCourante);
            int pos = 0;
            while (pos < tt.S.Length)
            {
                Vers v = new Vers(tt, pos, pwL);
                if (!zpCourante.AddVers(v))
                {
                    zpCourante = new ZonePoeme();
                    zL.Add(zpCourante);
                    if (!zpCourante.AddVers(v))
                    {
                        logger.Error("Une zone ne doit pas refuser le premier vers! {0}", v.ToString());
                        throw new InvalidOperationException("Une zone ne doit pas refuser le premier vers!");
                    }
                }
                pos = v.Last + 1;
            }

            // chercher d'éventuelles diérèses
            foreach (ZonePoeme zp in zL)
                zp.ChercheDierese(nbrPieds);
        }
    }
}
