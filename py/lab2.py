import math


def demo():

    # (weight, vertex1, vertex2)

    # graph_str = "(1,0,2) (1,0,3) (1,0,4) (2,1,2) (2,1,3) (2,2,4) (2,3,4)"
    # (edges, vertices) = parse_weighted(graph_str)
    # print(kruskal(edges, vertices))

    # unweighted_graph_str = "(0,1) (1,3) (0,2) (2,3)"
    # (edges, vertices) = parse_unweighted(unweighted_graph_str)
    # print(vertex_coloring(vertices))

    # graph_str = "(1,0,1) (1,0,2) (1,2,3) (1,1,3) (-2,3,0)"
    # (edges, vertices) = parse_weighted_digraph(graph_str)
    # print(bellman_ford(edges, vertices, 0))
    pass


def parse_weighted(graph_str):
    edges = [(float(edge[0]), int(edge[1]), int(edge[2]))
             for edge in [edge_str[1:-1].split(',') for edge_str in graph_str.split(' ')]]
    vertices = [set()
                for i in range(max([max(edge[1:]) for edge in edges]) + 1)]
    for edge in edges:
        vertices[edge[1]].add(edge[2])
        vertices[edge[2]].add(edge[1])
    return (edges, vertices)


def parse_weighted_digraph(graph_str):
    edges = [(float(edge[0]), int(edge[1]), int(edge[2]))
             for edge in [edge_str[1:-1].split(',') for edge_str in graph_str.split(' ')]]
    vertices = [set()
                for i in range(max([max(edge[1:]) for edge in edges]) + 1)]
    for edge in edges:
        vertices[edge[1]].add(edge[2])
    return (edges, vertices)


def parse_unweighted(graph_str):
    edges = [(int(edge[0]), int(edge[1]))
             for edge in [edge_str[1:-1].split(',') for edge_str in graph_str.split(' ')]]
    vertices = [set() for i in range(max([max(edge) for edge in edges]) + 1)]
    for edge in edges:
        vertices[edge[0]].add(edge[1])
        vertices[edge[1]].add(edge[0])
    return (edges, vertices)


def kruskal(edges, vertices):
    result = set()
    vertex_sets = [set([i]) for i in range(len(vertices))]
    edges.sort(key=lambda edge: edge[0])

    def assign_set(u, v):
        temp = vertex_sets[v]
        for i in range(len(vertices)):
            if vertex_sets[i] == temp:
                vertex_sets[i] = vertex_sets[u]

    for (w, u, v) in edges:
        if vertex_sets[u] != vertex_sets[v]:
            result.add((w, u, v))
            vertex_sets[u].update(vertex_sets[v])
            assign_set(u, v)

    return result


def vertex_coloring(vertices):

    def iteration(u: int, color_count: int, initial_coloring: list) -> (int, list):
        coloring = initial_coloring.copy()
        # for all neighbors
        for v in vertices[u]:
            # if vertex v is uncolored
            if coloring[v] == -1:
                possible_results = []
                # colors of all neighbors
                used_colors = set([coloring[v2]
                                   for v2 in vertices[v] if coloring[v2] != -1])
                # try every color up to color_count if it is unused
                for color in range(color_count):
                    if color not in used_colors:
                        coloring_copy = coloring.copy()
                        coloring_copy[v] = color
                        possible_results.append(
                            # recursion for the results
                            iteration(v, color_count, coloring_copy))

                # all colors used, introduce a new one
                if len(possible_results) == 0:
                    coloring[v] = color_count
                    color_count += 1
                    (color_count, coloring) = iteration(
                        v, color_count, coloring)
                else:
                    # take the coloring that used least amount of colors
                    (color_count, coloring) = min(
                        possible_results, key=lambda r: r[0])
        return (color_count, coloring)

    lis = [-1] * len(vertices)
    lis[0] = 0
    return iteration(0, 1, lis)


def invert_digraph(edges: list, vertices: list) -> (list, list):
    new_vertices = [[] for edge in edges]
    for i in range(len(new_vertices)):
        for edge in edges:
            if edge[0] == i:
                new_vertices[i].append(edge[1])
    new_edges = []
    for u in range(len(new_vertices)):
        for v in new_vertices[u]:
            new_edges.append((u, v))
    return (new_edges, new_vertices)


def bellman_ford(edges: list, vertices: list, source: int) -> (list, list):

    distance = [math.inf] * len(vertices)
    distance[source] = 0
    predecessor = [None] * len(vertices)

    for _ in range(len(vertices) - 1):
        for (w, u, v) in edges:
            if distance[u] + w < distance[v]:
                distance[v] = distance[u] + w
                predecessor[v] = u

    for (w, u, v) in edges:
        if distance[u] + w < distance[v]:
            raise Exception("The given graph contains a negative cycle")

    return (distance, predecessor)
