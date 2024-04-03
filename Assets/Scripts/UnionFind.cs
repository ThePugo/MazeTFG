using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//clase para manejar los conjuntos de celdas conectadas de forma unidimensional
public class UnionFind
{
    //'parent' almacena el índice del elemento padre de cada elemento.
    //Al inicio, cada elemento es su propio padre.
    private int[] parent;

    //'rank' ayuda a mantener el árbol balanceado. Un rango más alto indica un árbol más profundo.
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

    //Método Find: Encuentra el representante (raíz) del conjunto al que pertenece 'x'.
    //Utiliza compresión de caminos para aplanar la estructura del árbol, 
    //haciendo futuras búsquedas más rápidas.
    public int Find(int x)
    {
        if (parent[x] != x)
        {
            //Si 'x' no es su propio padre, busca recursivamente el padre de su padre,
            //al mismo tiempo que actualiza el padre de 'x' para apuntar directamente a la raíz.
            //Esto efectivamente acorta el camino de 'x' a su raíz.
            parent[x] = Find(parent[x]);
        }
        return parent[x];
    }

    //Método Union: Une los conjuntos que contienen a 'x' y 'y', respectivamente.
    public void Union(int x, int y)
    {
        //Encuentra las raíces de los conjuntos a los que pertenecen 'x' y 'y'.
        int rootX = Find(x);
        int rootY = Find(y);

        //Si tienen raíces diferentes, no están en el mismo conjunto, así que los unimos.
        if (rootX != rootY)
        {
            //Une los árboles según su rango para mantenerlos balanceados.
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

    // Método Connected: Verifica si 'x' y 'y' están en el mismo conjunto.
    public bool Connected(int x, int y)
    {
        // Si 'x' y 'y' tienen la misma raíz, están en el mismo conjunto.
        return Find(x) == Find(y);
    }
}