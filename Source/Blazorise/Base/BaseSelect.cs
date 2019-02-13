﻿#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Base
{
    public abstract class BaseSelect : BaseInputComponent
    {
        #region Members

        private string[] selectedValues;

        private List<BaseSelectItem> selectItems;

        #endregion

        #region Methods

        protected override void RegisterClasses()
        {
            ClassMapper
                .Add( () => ClassProvider.Select() )
                .If( () => ClassProvider.SelectSize( Size ), () => Size != Size.None )
                .If( () => ClassProvider.SelectValidation( ParentValidation?.Status ?? ValidationStatus.None ), () => ParentValidation?.Status != ValidationStatus.None );

            base.RegisterClasses();
        }

        protected override void OnInit()
        {
            // link to the parent component
            ParentValidation?.Hook( this );

            base.OnInit();
        }

        protected internal override void Dirty()
        {
            ClassMapper.Dirty();

            base.Dirty();
        }

        protected async void SelectionChangedHandler( UIChangeEventArgs e )
        {
            selectedValues = await JSRunner.GetSelectedOptions( ElementId );
            SelectedValueChanged?.Invoke( string.Join( ";", selectedValues ) );

            ParentValidation?.InputValueChanged( selectedValues );
        }

        internal void Register( BaseSelectItem selectItem )
        {
            if ( selectItem == null )
                return;

            if ( selectItems == null )
                selectItems = new List<BaseSelectItem>();

            if ( !selectItems.Contains( selectItem ) )
            {
                selectItems.Add( selectItem );

                ClassMapper.Dirty();

                //if ( selectItems?.Count > 1 ) // must find a better way to refresh
                //    StateHasChanged();
            }
        }

        internal bool IsSelected( BaseSelectItem selectItem )
        {
            return selectedValues?.Contains( selectItem?.Value ) == true;
        }

        #endregion

        #region Properties

        [Parameter] protected bool IsMultiple { get; set; }

        [Parameter]
        protected string SelectedValue
        {
            get
            {
                return string.Join( ";", selectedValues );
            }
            set
            {
                selectedValues = value?.Split( ';' );

                StateHasChanged();
            }
        }

        /// <summary>
        /// Occurs when the selected item value has changed.
        /// </summary>
        [Parameter] protected Action<string> SelectedValueChanged { get; set; }

        [CascadingParameter] protected BaseAddons ParentAddons { get; set; }

        [CascadingParameter] protected BaseValidation ParentValidation { get; set; }

        #endregion
    }
}
