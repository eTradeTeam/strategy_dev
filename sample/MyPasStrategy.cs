#region Using declarations
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Gui;
using NinjaTrader.Gui.Chart;
using NinjaTrader.Gui.SuperDom;
using NinjaTrader.Gui.Tools;
using NinjaTrader.Data;
using NinjaTrader.NinjaScript;
using NinjaTrader.Core.FloatingPoint;
using NinjaTrader.NinjaScript.Indicators;
using NinjaTrader.NinjaScript.DrawingTools;
using PriceActionSwing.Base;
#endregion

//This namespace holds Strategies in this folder and is required. Do not change it. 
namespace NinjaTrader.NinjaScript.Strategies
{
	public class MyPasStrategy : Strategy
	{
		private NinjaTrader.NinjaScript.Indicators.PriceActionSwing.PriceActionSwing PriceActionSwing1;
		private SimpleFont textFont = new SimpleFont("Courier", 15);
		private int SwingStrength;
		private Swing Swing1;
		
		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"Sample NT8 Strategy based on PriceActionSwing indicator. TF: 10min.";
				Name										= "MyPasStrategy";
				Calculate									= Calculate.OnBarClose;
				EntriesPerDirection							= 1;
				EntryHandling								= EntryHandling.AllEntries;
				IsExitOnSessionCloseStrategy				= true;
				ExitOnSessionCloseSeconds					= 30;
				IsFillLimitOnTouch							= false;
				MaximumBarsLookBack							= MaximumBarsLookBack.TwoHundredFiftySix;
				OrderFillResolution							= OrderFillResolution.Standard;
				Slippage									= 0;
				StartBehavior								= StartBehavior.WaitUntilFlat;
				TimeInForce									= TimeInForce.Gtc;
				TraceOrders									= false;
				RealtimeErrorHandling						= RealtimeErrorHandling.StopCancelClose;
				StopTargetHandling							= StopTargetHandling.PerEntryExecution;
				BarsRequiredToTrade							= 20;
				// Disable this property for performance gains in Strategy Analyzer optimizations
				// See the Help Guide for additional information
				IsInstantiatedOnEachOptimizationIteration	= true;
				SwingStrength								= 20;	// PAS
			}
			else if (State == State.Configure)
			{
			}
			else if (State == State.DataLoaded)
			{				
				Swing1 = Swing(Close, SwingStrength);
				PriceActionSwing1 = PriceActionSwing(Close, SwingStyle.Standard, 7, 20, false, SwingLengthStyle.Ticks,
					SwingDurationStyle.Bars, true,false, false, SwingTimeStyle.False,
					SwingVolumeStyle.Absolute, VisualizationStyle.Dots_ZigZag, textFont, 15, 30, 45,
					60, 75, 90,NinjaTrader.Gui.DashStyleHelper.Solid, 3, true, true);
				AddChartIndicator(PriceActionSwing1);
			}
		}

		protected override void OnBarUpdate()
		{
			if (BarsInProgress != 0 || CurrentBars[0] < 1 || CurrentBar < BarsRequiredToTrade) 
				return;
			
			 // When to go Long..
			if (PriceActionSwing1.DoubleBottom[0] != 0)
			{
				EnterLong(Convert.ToInt32(DefaultQuantity), "");
			}
			
			// When to get out..
			if (PriceActionSwing1.LowerHigh[0] !=0 && Position.MarketPosition == MarketPosition.Long)
					ExitLong(Convert.ToInt32(DefaultQuantity), @"ExitLong1a", "");
				
			
			
			// log data in output screen
			Print("Time: " + Time[0] + " |Number: " + CurrentBar + " | DB: " + PriceActionSwing1.DoubleBottom[0] +
			" | DT: " + PriceActionSwing1.DoubleTop[0] + " | HH: " + PriceActionSwing1.HigherHigh[0] +
			" | LH: " + PriceActionSwing1.LowerHigh[0] + " | LL: " + PriceActionSwing1.LowerLow[0] +
			" | HL: " + PriceActionSwing1.HigherLow[0] + " |  " );
			
		}
	}
}
