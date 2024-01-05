import { StyleSheet, ImageBackground } from 'react-native';
import { Text, View } from '../../components/Themed';
import { Link  } from 'expo-router';
import { Pressable, useColorScheme } from 'react-native';

export default function HomeScreen() {
  const colorScheme = useColorScheme();

  return (
    <ImageBackground
      source={require('../../assets/images/home.webp')} // Update with the correct path
      style={styles.background}
    >
      <View style={styles.container}>
        <Text style={styles.title}>Le voyage de Célia</Text>
        <Text style={styles.title}>L'anneau divin</Text>
        <Link href="/intro" asChild>
          <Pressable style={styles.button}>
            <Text style={styles.buttonText}>Introduction</Text>
          </Pressable>
        </Link>
        <Link href="/progression" asChild>
          <Pressable style={styles.button}>
            <Text style={styles.buttonText}>Commencer l'aventure</Text>
          </Pressable>
        </Link>
      </View>
    </ImageBackground>
  );
}

const styles = StyleSheet.create({
  background: {
    flex: 1,
    resizeMode: 'cover', // or 'stretch' or 'contain'
    height: '100%',
    width: '100%',
  },
  container: {
    flex: 1,
    alignItems: 'center',
    justifyContent: 'center',
  },
  title: {
    fontSize: 30,
    fontWeight: 'bold',
    fontFamily: 'Cinzel',
    textShadowColor: 'rgba(0, 0, 0, 1)',
    textShadowOffset: { width: -2, height: 2 },
    textShadowRadius: 3,
  },
  separator: {
    marginVertical: 30,
    height: 1,
    width: '80%',
  },
  button: {
    marginTop: 20,
    paddingVertical: 10,
    paddingHorizontal: 20,
    backgroundColor: '#8B4513',
    borderRadius: 5,
  },
  buttonText: {
    color: 'white',
    fontSize: 18,
    fontWeight: 'bold',
  },
});
