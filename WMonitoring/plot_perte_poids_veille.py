from plotly.subplots import make_subplots
import sportmember
import pandas as pd
import numpy as np
import plotly.express as px
import sportmember

def plot(df_poids, df_sport):
    df_poids['Date'] = pd.to_datetime(df_poids['Date'])
    df_poids['Date'] = df_poids['Date'].dt.strftime('%Y-%m-%d')
    df = pd.merge(df_poids, df_sport, on='Date', how='outer')
    df['SportValue'] = np.where(df['Sport'].isin(['Training', 'Game']), 1, 0)
    df['Sport'].fillna("None", inplace=True)
    df['Perte de poids'] = df['Poids'].diff()
    df['Signe'] = df['Perte de poids'].apply(lambda x: 'Positive' if x > 0 else ('Negative' if x < 0 else 'Nulle'))
    df['Perte de poids'].fillna(0, inplace=True)
    color_map = {'Positive': 'orange', 'Negative': 'green', 'Nulle': 'black'}
    fig = px.bar(df, x="Date", y="Perte de poids", color="Signe",
            labels={"Perte de poids": "Perte de poids par rapport à la veille (kg)", "Signe": "Variation"}, color_discrete_map=color_map)
    return fig
  
if __name__ == '__main__':
    df_poids = pd.read_csv("data.csv")
    df_sport = sportmember.get_sport()
    fig = plot(df_poids, df_sport)
    fig.show()