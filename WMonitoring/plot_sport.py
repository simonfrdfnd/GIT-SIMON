from plotly.subplots import make_subplots
import sportmember
import pandas as pd
import numpy as np
import plotly.express as px
import gsheet

def plot(df_poids, df_sport):
    df_poids['Date'] = pd.to_datetime(df_poids['Date'])
    df_poids['Date'] = df_poids['Date'].dt.strftime('%Y-%m-%d')
    df = pd.merge(df_poids, df_sport, on='Date', how='outer')
    df['SportValue'] = np.where(df['Sport'].isin(['Training', 'Game']), 1, 0)
    df['Sport'].fillna("None", inplace=True)
    color_map = {'Training': 'blue', 'Game': 'purple', 'None': 'black'}
    fig = px.bar(df, x="Date", y="SportValue", color="Sport",
            labels={"SportValue": "Sport"}, color_discrete_map=color_map)
    newnames = {"Training": "Entraînement", "Game": "Match", "None": "Aucun"}
    fig.for_each_trace(lambda t: t.update(name = newnames[t.name], 
                                          legendgroup = newnames[t.name], 
                                          hovertemplate = t.hovertemplate.replace(t.name, newnames[t.name])))
    return fig
    
if __name__ == '__main__':
    df_poids = pd.read_csv("data.csv")
    df_sport = sportmember.get_sport()
    fig = plot(df_poids, df_sport)
    fig.show()
