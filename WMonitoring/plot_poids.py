import pandas as pd
import plotly.express as px
import scipy.stats as stats
import numpy as np
import gsheet
from plotly.subplots import make_subplots
import plotly.graph_objects as go

def plot(df):
    fig = make_subplots(rows=1, cols=2, specs=[[{"type": "table"}, {"type": "scatter"}]])

    # Plot the line chart
    fig.add_scatter(x=df["Date"], y=df["Poids"], mode='lines', name='Poids (kg)', row=1, col=2)

    # Create a line from the starting weight to the goal weight (95 kg)
    start_weight = df["Poids"].iloc[0]
    start_date = pd.to_datetime(df["Date"].iloc[0])  # Convert the date to datetime
    goal_date = start_date + pd.DateOffset(months=5)
    date_range = pd.date_range(start_date, goal_date, freq='D')
    weights = np.linspace(start_weight, 95, num=len(date_range))

    line_data = pd.DataFrame({
        'Date': date_range,
        'Poids': weights
    })
    fig.add_scatter(x=line_data["Date"], y=line_data["Poids"], mode='lines', name='Tendance à suivre (kg)', line=dict(dash='dash'), row=1, col=2)

    # Perform linear regression
    slope, intercept, r_value, p_value, std_err = stats.linregress(df.index, df["Poids"])
    extended_date_range = pd.date_range(df["Date"].iloc[0], goal_date, freq='D')
    extended_weights = intercept + slope * np.arange(len(extended_date_range))
    regression_line = pd.DataFrame({
    'Date': extended_date_range,
    'Poids': extended_weights
    })

    fig.add_scatter(x=regression_line["Date"], y=regression_line["Poids"], mode='lines', name='Tendance (kg)', line=dict(dash='dash'), row=1, col=2)

    # Add a horizontal dashed line at 95 kilos
    weights = np.linspace(95, 95, num=len(date_range))
    line_data = pd.DataFrame({
        'Date': date_range,
        'Poids': weights
    })
    fig.add_scatter(x=line_data["Date"], y=line_data["Poids"], mode='lines', name='Objectif (kg)', row=1, col=2)

    # Set x-axis range to include the last date of the first curve
    fig.update_xaxes(title='Date', range=[df["Date"].iloc[0], df["Date"].iloc[-1]])
    fig.update_yaxes(title='Poids (kg)')
    fig.update_layout(title_text="Poids au court du temps")
    
    fig.add_table(cells=dict(values=[df['Date'], df['Poids']]), header=dict(values=["Date", "Poids (kg)"]), row=1, col=1)
    return fig

if __name__ == '__main__':
    # Load the CSV data
    df = pd.read_csv("data.csv")
    fig = plot(df)
    fig.show()