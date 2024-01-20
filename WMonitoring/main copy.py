import dash
import dash_core_components as dcc
import dash_html_components as html

import plotly.express as px
import pandas as pd
import os

# Load data
df = px.data.stocks()

fig = px.line(df, x="date", y="GOOG", template="plotly_dark").update_layout(
    {"plot_bgcolor": "rgba(0, 0, 0, 0)", "paper_bgcolor": "rgba(0, 0, 0, 0)"}
)

# Initialize the application
app = dash.Dash(__name__)

# define de app
app.layout = html.Div(
    children=[
        html.Div(
            className="row",
            children=[
                html.Div(
                    className="four columns div-user-controls",
                    children=[
                        html.H2("Dash - STOCK PRICES"),
                        html.P("""Visualising time series with Plotly - Dash"""),
                        html.P("""Pick one or more stocks from the dropdown below."""),
                    ],
                ),
                html.Div(
                    className="eight columns div-for-charts bg-grey",
                    children=[
                        dcc.Graph(
                            id="timeseries",
                            config={"displayModeBar": False},
                            animate=True,
                            figure=fig,
                        ),
                    ],
                ),
            ],
        )
    ]
)

# Run the app
if __name__ == "__main__":
    app.run_server(debug=True)