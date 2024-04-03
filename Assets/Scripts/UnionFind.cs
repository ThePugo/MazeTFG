using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//clase para manejar los conjuntos de celdas conectadas de forma unidimensional
public class UnionFind
{
    //'parent' almacena el �ndice del elemento padre de cada elemento.
    //Al inicio, cada elemento es su propio padre.
    private int[] parent;

    //'rank' ayuda a mantener el �rbol balanceado. Un rango m�s alto indica un �rbol m�s profundo.
    private int[] rank;

    //Constructor que inicializa las estructuras de datos.
    public UnionFind(int size)
    {
        parent = new int[size];
        rank = new int[size];
        //Inicializa cada elemento para que sea su propio padre.
        for (int i = 0; i < size; i++)
        {
            parent[i] = i;
        }
    }

    //M�todo Find: Encuentra el representante (ra�z) del conjunto al que pertenece 'x'.
    //Utiliza compresi�n de caminos para aplanar la estructura del �rbol, 
    //haciendo futuras b�squedas m�s r�pidas.
    public int Find(int x)
    {
        if (parent[x] != x)
        {
            //Si 'x' no es su propio padre, busca recursivamente el padre de su padre,
            //al mismo tiempo que actualiza el padre de 'x' para apuntar directamente a la ra�z.
            //Esto efectivamente acorta el camino de 'x' a su ra�z.
            parent[x] = Find(parent[x]);
        }
        return parent[x];
    }

    //M�todo Union: Une los conjuntos que contienen a 'x' y 'y', respectivamente.
    public void Union(int x, int y)
    {
        //Encuentra las ra�ces de los conjuntos a los que pertenecen 'x' y 'y'.
        int rootX = Find(x);
        int rootY = Find(y);

        //Si tienen ra�ces diferentes, no est�n en el mismo conjunto, as� que los unimos.
        if (rootX != rootY)
        {
            //Une los �rboles seg�n su rango para mantenerlos balanceados.
            if (rank[rootX] < rank[rootY])
            {
                //Si el rango de rootX es menor que el de rootY, hace rootY el nuevo padre de rootX.
                parent[rootX] = rootY;
            }
            else if (rank[rootX] > rank[rootY])
            {
                //Si el rango de rootX es mayor, hace rootX el nuevo padre de rootY.
                parent[rootY] = rootX;
            }
            else
            {
                //Si los rangos son iguales, simplemente elige uno como padre y incrementa su rango.
                parent[rootY] = rootX;
                rank[rootX]++;
            }
        }
    }

    // M�todo Connected: Verifica si 'x' y 'y' est�n en el mismo conjunto.
    public bool Connected(int x, int y)
    {
        // Si 'x' y 'y' tienen la misma ra�z, est�n en el mismo conjunto.
        return Find(x) == Find(y);
    }
}