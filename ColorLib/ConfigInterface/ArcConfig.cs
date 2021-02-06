/********************************************************************************
 *  Copyright 2020, Pierre-Alain Etique                                         *
 *                                                                              *
 *  This file is part of Coloriƨation.                                          *
 *                                                                              *
 *  Coloriƨation is free software: you can redistribute it and/or modify        *
 *  it under the terms of the GNU General Public License as published by        *
 *  the Free Software Foundation, either version 3 of the License, or           *
 *  (at your option) any later version.                                         *
 *                                                                              *
 *  Coloriƨation is distributed in the hope that it will be useful,             *
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of              *
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the               *
 *  GNU General Public License for more details.                                *
 *                                                                              *
 *  You should have received a copy of the GNU General Public License           *
 *  along with Coloriƨation.  If not, see <https://www.gnu.org/licenses/>.      *
 *                                                                              *
 ********************************************************************************/

using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Text;

namespace ColorLib
{
    public class ArcButtonModifiedEventArgs : EventArgs
    {
        public int buttonNr { get; private set; }

        public ArcButtonModifiedEventArgs(int inBNr)
        {
            buttonNr = inBNr;
        }
    }

    /// <summary>
    /// Configuration pour les arcs
    /// </summary>
    [Serializable]
    public class ArcConfig: ConfigBase
    {

        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        [Serializable]
        public struct ArcButtonConf
        {
            public bool buttonClickable;
            public CharFormatting cf;
        }

        /// <summary>
        /// Nombre de boutons gérés. 
        /// </summary>
        public const int NrArcButtons = 6;

        // -------------------------------------------------------------------------------------------------------------------
        // ----------------------------------------------  Event Handlers ----------------------------------------------------
        // -------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Evènement déclenché quand le bouton pour couleur est modifié.
        /// </summary>
        [field: NonSerialized]
        public event EventHandler<ArcButtonModifiedEventArgs> ArcButtonModified;

        /// <summary>
        /// Evènement déclenché quand la hauteur est modifiée.
        /// </summary>
        [field: NonSerialized]
        public event EventHandler HauteurModified;

        /// <summary>
        /// Evènement déclenché quand l'écartement est modifié.
        /// </summary>
        [field: NonSerialized]
        public event EventHandler EcartementModified;

        /// <summary>
        /// Evènement déclenché quand l'épaisseur est modifiée.
        /// </summary>
        [field: NonSerialized]
        public event EventHandler EpaisseurModified;

        /// <summary>
        /// Evènement déclenché quand le décalage est modifié.
        /// </summary>
        [field: NonSerialized]
        public event EventHandler DecalageModified;

        // -------------------------------------------------------------------------------------------------------------------
        // ----------------------------------------------  Public Members ---------------------------------------------------
        // -------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Hauteur des arcs en pourcentage de la hauteur de la ligne
        /// </summary>
        public int Hauteur
        {
            get
            {
                return _hauteur;
            }
            set
            {
                if (_hauteur != value) // pour ne déclencher l'évèenement que s'il y a vraiement un changement.
                {
                    _hauteur = value;
                    OnHauteurModified();
                }
            }
        }

        /// <summary>
        /// Donne en pourcents l'écartement des points de guidage de le courbe de Bézier. A 100%
        /// les points de tangeance au départ) sont verticaux. En réduisant le nombre, la tangente
        /// s'applatit. 
        /// </summary>
        public int Ecartement // en pourcents
        {
            get
            {
                return _ecartement;
            }
            set
            {
                if (value != _ecartement)
                {
                    _ecartement = value;
                    OnEcartementModified();
                }
            }
        }

        /// <summary>
        /// Epaisseur des arcs.
        /// </summary>
        public float Epaisseur // en pourcents
        {
            get
            {
                return _epaisseur;
            }
            set
            {
                if (value != _epaisseur)
                {
                    _epaisseur = value;
                    OnEpaisseurModified();
                }
            }
        }

        /// <summary>
        /// Décalage en points des arcs par rapport au texte souligné. Une valeur positive éloigne
        /// du texte, une valeur négative rapproche.
        /// </summary>
        public float Decalage // en pourcents
        {
            get
            {
                return _decalage;
            }
            set
            {
                if (value != _decalage)
                {
                    _decalage = value;
                    OnDecalageModified();
                }
            }
        }

        // -------------------------------------------------------------------------------------------------------------------
        // ----------------------------------------------  Private Members ---------------------------------------------------
        // -------------------------------------------------------------------------------------------------------------------

        private ArcButtonConf[] arcButtons;
        private int nrSetArcButtons;
        private int counter;

        private int _hauteur;
        private int _ecartement;
        private float _epaisseur;
        private float _decalage;

        // -------------------------------------------------------------------------------------------------------------------
        // ----------------------------------------------  Public Methods ----------------------------------------------------
        // -------------------------------------------------------------------------------------------------------------------

        public ArcConfig()
        {
            arcButtons = new ArcButtonConf[NrArcButtons];
            Reset();
        }

        /// <summary>
        /// Indique s'il s'agit du bouton qui n'est pas encore actif, mais qu'on peut configurer.
        /// </summary>
        /// <param name="butNr"></param>
        /// <returns></returns>
        public bool ButtonIsActivableOne(int butNr) => butNr == nrSetArcButtons;

        /// <summary>
        /// Indique si le bouton donné est le dernier bouton actif, c.à.d. si c'est celui qui peut
        /// être effacé.
        /// </summary>
        /// <param name="butNr">Le bouton à analyser.</param>
        /// <returns><c>true</c>: le bouton est le dernier actif.</returns>
        public bool ButtonIsLastActive(int butNr) => butNr == nrSetArcButtons - 1;

        /// <summary>
        /// Donne la <c>ArcButtonConf</c> pour <c>butNr</c>.
        /// </summary>
        /// <param name="butNr">Le numéro du boutonpour lequel on veut connaître la conf.</param>
        /// <returns>La <c>ArcButtonConf</c> cherchée.</returns>
        public ArcButtonConf GetArcButtonConfFor(int butNr) => arcButtons[butNr];

        /// <summary>
        /// Indique que le le boouton <paramref name="butNr"/> (commence à 0) doit être formatté avec 
        /// <paramref name="inCol"/>.
        /// La fonction <c>updateArcButton</c> est appelée pour le bouton <paramref name="butNr"/> (l'événement
        /// correspondant est généré...)
        /// </summary>
        /// <param name="butNr">Le numéro du bouton dont il faut changer le formatage.</param>
        /// <param name="inCol">Le nouveau formatage.</param>
        public void SetArcButtonCol(int butNr, RGB inCol)
        {
            logger.ConditionalDebug("SetArcButtonCol butNr: {0}, Col: {1}", butNr, inCol);
            if (butNr > nrSetArcButtons)
            {
                logger.Fatal("Modification d'un bouton d'arc non actif butNr: {0}", butNr);
                throw new ArgumentException("Modification d'un bouton d'arc non actif.", nameof(butNr));
            }
            arcButtons[butNr].cf = new CharFormatting(true, inCol);
            if (butNr == nrSetArcButtons)
            {
                nrSetArcButtons++;
                if (nrSetArcButtons < NrArcButtons)
                {
                    arcButtons[nrSetArcButtons].buttonClickable = true;
                    OnArcButtonModified(nrSetArcButtons);
                }
            }
            OnArcButtonModified(butNr);
        }

        /// <summary>
        /// Définit un nouveau <c>CharFormatting</c> pour le bouton indiqué.
        /// </summary>
        /// <param name="butNrTxt">Le numéro du boutton.</param>
        /// <param name="inCol">La nouvelle couleur pour le bouton.</param>
        public void SetArcButtonCol(string butNrTxt, RGB inCol)
        {
            SetArcButtonCol(int.Parse(butNrTxt), inCol);
        }

        /// <summary>
        /// Le bouton <paramref name="butNr"/> est effacé. N'est possible que pour le dernier bouton
        /// formaté de la série.
        /// </summary>
        /// <exception cref="ArgumentException"> est levée si le bouton ne peut pas être effacé.</exception>
        /// <param name="butNr">Le numéro du bouton à effacer.</param>
        public void ClearButton(int butNr)
        {
            logger.ConditionalDebug("ClearButton butNr: {0}", butNr);
            if ((butNr == (nrSetArcButtons - 1)) && (nrSetArcButtons > 0))
            {
                arcButtons[nrSetArcButtons].buttonClickable = false;
                OnArcButtonModified(nrSetArcButtons);
                nrSetArcButtons--;
                arcButtons[nrSetArcButtons].cf = CharFormatting.NeutralCF;
                OnArcButtonModified(nrSetArcButtons);
            }
            else
            {
                Exception e = new ArgumentException("The Button Number cannot be cleared", "butNr");
                logger.Error(e, "The Button number {0} cannot be cleared. nrSetButtons: {1}", butNr, nrSetArcButtons);
                throw e;
            }
        }
        
        /// <summary>
        /// Retourne le <see cref="CharFormatting"/> à utiliser en alternance suivant la configuration actuelle.
        /// Assure de passer à travers les différents <c>CharFormatting</c> voulus.
        /// </summary>
        /// <remarks>
        /// Si aucun bouton n'a été défini, retourne le <c>CharFormatting</c> neutre.
        /// </remarks>
        /// <returns>Le <see cref="CharFormatting"/> à utiliser pour un formatage alterné.</returns>
        public CharFormatting NextCF()
        {
            CharFormatting toReturn = arcButtons[counter].cf;
            if (nrSetArcButtons > 0)
                counter = (counter + 1) % nrSetArcButtons;
            else
                counter = 0;
            return toReturn;
        }

        /// <summary>
        /// Réinitialise le compteur utilisé par <see cref="NextCF"/> pour retourner les formatages aletrnés. 
        /// Permet de s'assurer que la série retournée commence par le formatage du premier bouton.
        /// </summary>
        public void ResetCounter() => counter = 0;

        /// <summary>
        /// Réinitialise à la configuration par défaut (rouge et noir)
        /// </summary>
        public override void Reset()
        {
            for (int i = 1; i < NrArcButtons; i++)
            {
                arcButtons[i].buttonClickable = false;
                arcButtons[i].cf = CharFormatting.NeutralCF;
                OnArcButtonModified(i);
            }
            arcButtons[0].buttonClickable = true;
            nrSetArcButtons = 0;
            SetArcButtonCol(0, ColConfWin.predefinedColors[(int)PredefCol.darkBlue]);
            ResetCounter();
            _hauteur = 65;
            _ecartement = 75;
            _epaisseur = 0.5f;
            _decalage = 0.0f;
        }

        // --------------------------------------- Serialization ----------------------------------

        [OnDeserializing()]
        private void SetOptionalFieldsToDefaultVal(StreamingContext sc)
        {
            logger.ConditionalDebug("SetOptionalFieldsToDefaultVal");
        }

        // ------------------------------------------ Events --------------------------------------

        protected virtual void OnArcButtonModified(int buttonNr)
        {
            logger.ConditionalDebug(ConfigBase.cultF, "OnArcButtonModified bouton: \'{0}\'", buttonNr);
            EventHandler<ArcButtonModifiedEventArgs> eventHandler = ArcButtonModified;
            eventHandler?.Invoke(this, new ArcButtonModifiedEventArgs(buttonNr));
        }

        protected virtual void OnHauteurModified()
        {
            logger.ConditionalDebug("OnHauteurModified");
            EventHandler eventHandler = HauteurModified;
            eventHandler?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnEcartementModified()
        {
            logger.ConditionalDebug("OnEcartementModified");
            EventHandler eventHandler = EcartementModified;
            eventHandler?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnEpaisseurModified()
        {
            logger.ConditionalDebug("OnEpaisseurModified");
            EventHandler eventHandler = EpaisseurModified;
            eventHandler?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnDecalageModified()
        {
            logger.ConditionalDebug("OnDecalageModified");
            EventHandler eventHandler = DecalageModified;
            eventHandler?.Invoke(this, EventArgs.Empty);
        }

    }
}
