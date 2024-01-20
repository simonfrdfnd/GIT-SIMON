import dash
from dash import dcc
from dash import html
import plotly.express as px
import pandas as pd
import plot_poids
import plot_perte_poids
import plot_perte_poids_veille
import plot_sport
import sportmember

# Load data
df = px.data.stocks()

df_poids = pd.read_csv("data.csv")
df_sport = sportmember.get_sport()
fig0 = plot_poids.plot(df_poids).update_layout(template="plotly_dark")
fig1 = plot_perte_poids.plot(df_poids, df_sport).update_layout(template="plotly_dark")
fig2 = plot_perte_poids_veille.plot(df_poids, df_sport).update_layout(template="plotly_dark")
fig3 = plot_sport.plot(df_poids, df_sport).update_layout(template="plotly_dark")

# Initialize the application
app = dash.Dash(__name__)

# define de app
app.layout = html.Div(
    children=[
        html.Div(
            dcc.Graph(
                id="graph0",
                animate=True,
                figure=fig0,
            )),
        html.Div(
            dcc.Graph(
                id="graph1",
                animate=True,
                figure=fig1,
            )),
        html.Div(
            dcc.Graph(
                id="graph2",
                animate=True,
                figure=fig2,
            )),
        html.Div(
            dcc.Graph(
                id="graph3",
                animate=True,
                figure=fig3,
            ))
    ]
)

# Run the app
if __name__ == "__main__":
    app.run_server(debug=True)